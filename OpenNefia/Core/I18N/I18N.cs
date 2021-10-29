using OpenNefia.Core.Extensions;
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
        // TODO needs to be LanguageDef
        public static string Language { get => "ja_JP"; }

        public static string Get(LocaleKey key)
        {
            return key;
        }        

        public static bool IsFullwidth()
        {
            return Language == "ja_JP";
        }

        public static void DoLocalize(ILocalizable o, LocaleKey key)
        {
            foreach (var field in o.GetLocalizableFields())
            {
                var attr = field.GetLocalizeAttribute();

                if (field.FieldType == typeof(string))
                {
                    field.SetValue(o, Get(key));
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

        public static IEnumerable<FieldInfo> GetLocalizableFields(this ILocalizable o)
        {
            return o.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(field => field.GetLocalizeAttribute() != null);
        }
    }
}
