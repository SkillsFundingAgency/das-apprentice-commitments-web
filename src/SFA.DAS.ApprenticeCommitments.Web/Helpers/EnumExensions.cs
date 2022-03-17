using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace SFA.DAS.ApprenticeCommitments.Web.Helpers
{
    public static class EnumExtensions
    {
        public static string DisplayName(this Enum enumValue)
        {
            var displayName =
                GetDisplayAttribute(enumValue)
                ?.GetName();

            return string.IsNullOrEmpty(displayName)
                ? enumValue.ToString()
                : displayName;
        }

        private static DisplayAttribute? GetDisplayAttribute(Enum enumValue)
            => enumValue.GetType()
                    .GetMember(enumValue.ToString())
                    .FirstOrDefault()
                    .GetCustomAttribute<DisplayAttribute>();
    }
}