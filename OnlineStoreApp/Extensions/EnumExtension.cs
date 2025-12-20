using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace OnlineStoreApp.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum value)
        {
            var memberInfo = value
                .GetType()
                .GetMember(value.ToString())
                .FirstOrDefault();

            if (memberInfo == null)
                return value.ToString();

            var displayAttribute =
                memberInfo.GetCustomAttribute<DisplayAttribute>();

            return displayAttribute?.Name ?? value.ToString();
        }
    }
}
