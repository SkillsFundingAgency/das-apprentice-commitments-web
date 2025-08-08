using System;
using System.ComponentModel;
using System.Reflection;
using SFA.DAS.ApprenticeCommitments.Web.Models;

namespace SFA.DAS.ApprenticeCommitments.Web.Helpers
{
    public static class Utilities
    {
        public static string GetApprenticeshipTypeDescription(int? value)
        {
            if (value != null)
            {
                ApprenticeshipType enumValue = (ApprenticeshipType)value;
                var enumName = enumValue.ToString();

                FieldInfo? field = typeof(ApprenticeshipType).GetField(enumName);

                if (field != null)
                {
                    DescriptionAttribute? attribute = field.GetCustomAttribute<DescriptionAttribute>();
                    if (attribute != null)
                    {
                        return attribute.Description;
                    }
                }

                return enumName;
            }
            return "";
        }
        
        public static bool IsValidApprenticeshipType(int? value)
        {
            return value.HasValue && Enum.IsDefined(typeof(ApprenticeshipType), value.Value);
        }
    }
}