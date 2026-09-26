using System.ComponentModel;
using System.Reflection;

namespace FeedbackApi.Domain.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            DescriptionAttribute attribute = field?.GetCustomAttribute<DescriptionAttribute>();
            return attribute?.Description ?? value.ToString();
        }

        public static TEnum ToEnum<TEnum>(this string value, TEnum defaultValue = default) where TEnum : struct, Enum
        {
            return Enum.TryParse(value, true, out TEnum result) ? result : defaultValue;
        }
    }
}
