using OpenNefia.Core.Data.Serial;
using OpenNefia.Core.Extensions;
using OpenNefia.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        }

        public static string GetString(LocaleKey key)
        {
            var result = Env.Store.GetValueOrDefault(key);
            if (result != null)
            {
                return result;
            }
            return $"<Missing key: {key}>";
        }        

        public static bool IsFullwidth()
        {
            return Language == "ja_JP";
        }

        public static void DoLocalize(object o, LocaleKey key)
        {
            foreach (var field in o.GetLocalizableFields())
            {
                var attr = field.GetLocalizeAttribute();

                if (field.FieldType == typeof(string))
                {
                    field.SetValue(o, GetString(key.With(field.Name)));
                }
                else if (typeof(Delegate).IsAssignableFrom(field.FieldType))
                {
                }
                else if (typeof(ILocalizable).IsAssignableFrom(field.FieldType))
                {
                    var localizable = (ILocalizable)field.GetValue(o)!;
                    var keyFrag = attr?.Key ?? field.Name;
                    localizable.Localize(key.With(keyFrag));
                }
                else
                {
                    throw new Exception($"Cannot localize field of type {field.FieldType}");
                }
            }
        }

        public static ILocalizeAttribute? GetLocalizeAttribute(this MemberInfo member)
        {
            return member.GetCustomAttributes()
                .Select(attr => attr as ILocalizeAttribute)
                .WhereNotNull()
                .FirstOrDefault();
        }

        public static IEnumerable<FieldInfo> GetLocalizableFields(this object o)
        {
            return o.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(field => field.GetLocalizeAttribute() != null);
        }
    }
}
