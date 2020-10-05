using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NationalInstruments.Utilities.SignalCreator
{
    public enum MemberAccessibility { All, Writeable, Readable }
    public static class Extensions
    {
        public static IEnumerable<MemberInfo> GetPropertiesAndFields<T>()
        {
            return typeof(T).GetPropertiesAndFields();
        }
        public static IEnumerable<MemberInfo> GetPropertiesAndFields(this Type t, MemberAccessibility accessiblity = MemberAccessibility.All)
        {
            BindingFlags searchFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy;

            IEnumerable<MemberInfo> fields = t.GetFields(searchFlags);
            IEnumerable<PropertyInfo> properties = t.GetProperties(searchFlags);
            switch (accessiblity)
            { 
                case MemberAccessibility.Readable:
                    properties = properties.Where(p => p.CanRead);
                    break;
                case MemberAccessibility.Writeable:
                    properties = properties.Where(p => p.CanWrite);
                    break;
            }
            return properties.Concat(fields);
        }
        public static object GetValue(this MemberInfo member, object containingObject)
        {
            switch (member)
            {
                case FieldInfo f:
                    return f.GetValue(containingObject);
                case PropertyInfo p:
                    return p.GetValue(containingObject);
                default:
                    throw new ArgumentException($"Paramter must be a {typeof(FieldInfo)} or a {typeof(MemberInfo)}.", nameof(member));
            }
        }
        public static void SetValue(this MemberInfo member, object containingObject, object value)
        {
            switch (member)
            {
                case FieldInfo f:
                    f.SetValue(containingObject, value);
                    break;
                case PropertyInfo p:
                    p.SetValue(containingObject, value);
                    break;
                default:
                    throw new ArgumentException($"Paramter must be a {typeof(FieldInfo)} or a {typeof(MemberInfo)}.", nameof(member));
            }
        }
        public static Type GetMemberType(this MemberInfo member)
        {
            switch (member)
            {
                case FieldInfo f:
                    return f.FieldType;
                case PropertyInfo p:
                    return p.PropertyType;
                default:
                    throw new ArgumentException($"Paramter must be a {typeof(FieldInfo)} or a {typeof(MemberInfo)}.", nameof(member));
            }
        }



        // From https://www.extensionmethod.net/csharp/type/issubclassofrawgeneric
        /// <summary>
        /// Alternative version of <see cref="Type.IsSubclassOf"/> that supports raw generic types (generic types without
        /// any type parameters).
        /// </summary>
        /// <param name="baseType">The base type class for which the check is made.</param>
        /// <param name="toCheck">To type to determine for whether it derives from <paramref name="baseType"/>.</param>
        public static bool IsSubclassOfRawGeneric(this Type toCheck, Type baseType)
        {
            while (toCheck != typeof(object))
            {
                Type cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (baseType == cur)
                {
                    return true;
                }

                toCheck = toCheck.BaseType;
            }

            return false;
        }
        public static bool IsDefined<T>(this MemberInfo member)
        {
            return member.IsDefined(typeof(T));
        }
    }
}
