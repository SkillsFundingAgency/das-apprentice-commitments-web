using System;
using SFA.DAS.ApprenticeCommitments.Types;

namespace SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi
{
    public class Apprenticeship
    {
        public long Id { get; set; }
        public long RevisionId { get; set; }
        public string EmployerName { get; set; } = null!;
        public string TrainingProviderName { get; set; } = null!;
        public bool? EmployerCorrect { get; set; }
        public bool? TrainingProviderCorrect { get; set; }
        public bool? RolesAndResponsibilitiesCorrect { get; set; }
        public bool? ApprenticeshipDetailsCorrect { get; set; }
        public bool? HowApprenticeshipDeliveredCorrect { get; set; }
        public DateTime ConfirmBefore { get; set; }
        public DateTime? ConfirmedOn { get; set; }
        public DateTime? LastViewed { get; set; }
        public string CourseName { get; set; } = null!;
        public int CourseLevel { get; set; }
        public string? CourseOption { get; set; } = null!;
        public int CourseDuration { get; set; }
        public int DurationInMonths { get; set; }
        public DateTime PlannedStartDate { get; set; }
        public DateTime PlannedEndDate { get; set; }
        public ChangeOfCircumstanceNotifications ChangeOfCircumstanceNotifications { get; set; }
    }
}