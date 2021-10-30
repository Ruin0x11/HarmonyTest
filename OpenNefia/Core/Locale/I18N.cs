using NLua;
using OpenNefia.Core.Data.Serial;
using OpenNefia.Core.Extensions;
using OpenNefia.Core.Util;
using OpenNefia.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core
{
    public static class I18N
    {
        internal static LocalizationEnv Env = null!;

        internal static void InitStaticGlobals()
        {
            Env = new LocalizationEnv();
        }

        // TODO needs to be LanguageDef
        public static string Language { get; private set; } = "en_US";
        
        public static void SwitchLanguage(string language)
        {
            Language = language;
            Env.Clear();
            Env.LoadAll(language);

            foreach (var layer in Engine.Instance.Layers)
            {
                layer.Localize(layer.GetType()!.FullName!);
            }

            DefLoader.LocalizeDefs();
            LocalizeStaticFields();
        }

        public static string GetString(LocaleKey key)
        {
            var result = Env.StringStore.GetValueOrDefault(key);
            if (result != null)
            {
                return result;
            }
            return $"<Missing key: {key}>";
        }

        public static LocaleFunc<T> GetFunction<T>(LocaleKey key)
        {
            var luaFunc = Env.FunctionStore.GetValueOrDefault(key);
            if (luaFunc == null)
            {
                var luaString = Env.StringStore.GetValueOrDefault(key);
                if (luaString == null)
                {
                    return (_) => $"<Missing key: {key}>";
                }
                else
                {
                    return (_) => luaString;
                }
            }

            return (arg) =>
            {
                var result = luaFunc.Call(arg)[0];
                return $"{result}";
            };
        }

        public static LocaleFunc<T1, T2> GetFunction<T1, T2>(LocaleKey key)
        {
            var luaFunc = Env.FunctionStore.GetValueOrDefault(key);
            if (luaFunc == null)
            {
                var luaString = Env.StringStore.GetValueOrDefault(key);
                if (luaString == null)
                {
                    return (_, _) => $"<Missing key: {key}>";
                }
                else
                {
                    return (_, _) => luaString;
                }
            }

            return (arg1, arg2) =>
            {
                var result = luaFunc.Call(arg1, arg2)[0];
                return $"{result}";
            };
        }

        internal static void LocalizeStaticFields()
        {
            foreach (var type in ReflectionUtils.AllTypes)
            {
                var baseKey = type.GetBaseLocaleKey();
                foreach (var field in type.GetLocalizableStaticFields())
                {
                    DoLocalizeField(null, baseKey, field);
                }
            }
        }

        public static LocaleKey GetBaseLocaleKey(this Type type) => type.FullName!;

        public static bool IsFullwidth()
        {
            return Language == "ja_JP";
        }

        public static void DoLocalize(object o, LocaleKey key)
        {
            foreach (var field in o.GetType().GetLocalizableFields())
            {
                DoLocalizeField(o, key, field);
            }
        }

        private static void DoLocalizeField(object? o, LocaleKey baseKey, FieldInfo field)
        {
            var attr = field.GetLocalizeAttribute();

            // If the field is an ILocalizable, the locale key will match exactly how it is defined
            // in Lua.

            // Otherwise, the name of the field must be "Text<...>" in the C# side and the key is
            // "<...>" in Lua.
            // This makes it easier to tell what was localized from the variable name and what is
            // just a plain string.

            string keyFrag;

            if (typeof(ILocalizable).IsAssignableFrom(field.FieldType))
            {
                keyFrag = attr?.Key ?? field.Name;
                var localizable = (ILocalizable)field.GetValue(o)!;
                localizable.Localize(baseKey.With(keyFrag));
                return;
            }

            if (field.IsStatic)
            {
                if (field.Name.StartsWith("Text"))
                {
                    keyFrag = attr?.Key ?? field.Name.RemovePrefix("Text");
                }
                else
                {
                    throw new Exception($"Localized fields should start with 'Text' (got: {field.Name}, {baseKey})");
                }
            }
            else
            {
                keyFrag = attr?.Key ?? field.Name;
            }

            var nextKey = baseKey.With(keyFrag);

            if (field.FieldType == typeof(string))
            {
                field.SetValue(o, GetString(nextKey));
            }
            else if (field.FieldType.IsGenericType)
            {
                var genericType = field.FieldType.GetGenericTypeDefinition();
                if (genericType == typeof(Dictionary<,>))
                {
                    LocalizeDictionary(field, nextKey);
                }
                else if (genericType == typeof(LocaleFunc<>) || genericType == typeof(LocaleFunc<,>))
                {
                    var method = GetFunctionMethod(field);
                    if (method == null)
                    {
                        throw new Exception($"Cannot localize to function of type {field.FieldType}");
                    }
                    field.SetValue(o, method.Invoke(null, new object[] { nextKey }));
                }
                else
                {
                    throw new Exception($"Cannot localize field of generic type {field.FieldType}");
                }
            }
            else
            {
                throw new Exception($"Cannot localize field of type {field.FieldType}");
            }
        }

        private static MethodInfo? GetFunctionMethod(FieldInfo field)
        {
            var genericArgs = field.FieldType.GetGenericArguments();
            return typeof(I18N).GetMethods().FirstOrDefault(
                x => x.Name.Equals(nameof(GetFunction)) &&
                x.IsGenericMethod && x.GetGenericArguments().Length == genericArgs.Length)
                ?.MakeGenericMethod(genericArgs);
        }

        private static void LocalizeDictionary(FieldInfo field, LocaleKey localeKey)
        {
            var luaTable = Env.Lua.GetTable(localeKey);
            
            if (luaTable == null)
            {
                throw new Exception($"Lua table at key {localeKey} not found.");
            }

            var ty = field.FieldType;
            var keyTy = ty.GetGenericArguments()[0];
            var valueTy = ty.GetGenericArguments()[1];

            var dict = Activator.CreateInstance(ty)!;

            var methodAdd = dict.GetType().GetMethod("Add")!;
            foreach (KeyValuePair<object, object> pair in luaTable)
            {
                object key = Convert.ChangeType(pair.Key, keyTy);
                object value;

                if (valueTy == typeof(string))
                {
                    value = pair.Value.ToString()!;
                }
                else if (valueTy.IsGenericType && valueTy.GetGenericTypeDefinition() == typeof(LocaleFunc<>))
                {
                    var method = typeof(I18N).GetMethod(nameof(MakeLocaleFunc), BindingFlags.Static | BindingFlags.NonPublic)!.MakeGenericMethod(valueTy.GetGenericArguments());
                    value = method.Invoke(null, new object[] { pair.Value, valueTy })!;
                }
                else
                {
                    throw new Exception($"Cannot convert to a localized object of type {valueTy}");
                }

                methodAdd.Invoke(dict, new object?[] { key, value });
            }

            field.SetValue(null, dict);
        }

        private static LocaleFunc<T> MakeLocaleFunc<T>(object luaValue, Type localeFuncType)
        {
            var luaFunc = luaValue as LuaFunction;
            if (luaFunc == null)
            {
                var str = luaValue.ToString()!;
                return (_) => str;
            }
            return (arg) =>
            {
                var result = luaFunc.Call(arg)[0];
                return $"{result}";
            };
        }

        public static ILocalizeAttribute? GetLocalizeAttribute(this MemberInfo member)
        {
            return member.GetCustomAttributes()
                .Select(attr => attr as ILocalizeAttribute)
                .WhereNotNull()
                .FirstOrDefault();
        }

        public static IEnumerable<FieldInfo> GetLocalizableFields(this Type type)
        {
            return type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(field => field.GetLocalizeAttribute() != null);
        }

        public static IEnumerable<FieldInfo> GetLocalizableStaticFields(this Type type)
        {
            return type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(field => field.GetLocalizeAttribute() != null);
        }
    }
}
