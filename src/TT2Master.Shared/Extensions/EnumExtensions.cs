using System;
using System.ComponentModel;

namespace TT2Master.Shared.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Gets description value as string for an enum value.
        /// </summary>
        /// <param name="value">This enum</param>
        /// <returns>Description attribute value as string.</returns>
        public static string GetDescription(this Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                System.Reflection.FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attr)
                    {
                        return attr.Description;
                    }
                }
            }
            return null;
        }
    }
}
