using System.Linq;
using System.Reflection;

namespace Vexe.Runtime.Extensions
{
    public static class PropertyInfoExtensions
    {
        public static bool IsAutoProperty(this PropertyInfo pinfo)
        {
            // first make sure the property has both a getter and setter
            if (!(pinfo.CanWrite && pinfo.CanWrite))
                return false;

            // then, check to make sure there's a complier generated backing field for the property
            // the backing field would be something like: "<PropName>k__BackingField;
            // and would have a [CompilerGenerated] attribute on it. but checking for the name is enough
            // it would also be private on an instance basis
            var flag = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
            string compilerGeneratedName = "<" + pinfo.Name + ">";
            return pinfo.DeclaringType.GetFields(flag).Any(f => f.Name.Contains(compilerGeneratedName));
        }

        public static bool IsIndexer(this PropertyInfo property)
        {
            return property.GetIndexParameters().Length > 0;
        }

        public static bool CanReadWrite(this PropertyInfo property)
        {
            return property.CanRead && property.CanWrite;
        }
    }
}