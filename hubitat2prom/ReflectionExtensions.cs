using System;
using System.Reflection;

namespace hubitat2prom
{
    public static class ReflectionExtensions
    {
        public static string FriendlyName(this object obj)
            => FriendlyName(obj.GetType());

        public static string FriendlyName(this PropertyInfo propertyInfo)
            => FriendlyName(propertyInfo.PropertyType);

        public static string FriendlyName(this Type type)
        {
            var name = "";
            if (type.Namespace != null)
            {
                name += type.Namespace + ".";
            }
            name += type.Name;

            if (!type.IsGenericType) return name;

            name += "[";

            var seperator = "";

            foreach (var t in type.GetGenericArguments())
            {
                name += seperator + FriendlyName(t);
                seperator = ", ";
            }

            name += "]";

            return name;
        }
    }
}
