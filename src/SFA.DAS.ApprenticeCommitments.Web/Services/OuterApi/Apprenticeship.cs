using System;

namespace SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi
{
    public class Apprenticeship
    {
        public long Id { get; set; }
        public string EmployerName { get; set; }
        public string TrainingProviderName { get; set; }
        public bool? EmployerCorrect { get; set; }
        public bool? TrainingProviderCorrect { get; set; }
        public bool? ApprenticeshipDetailsCorrect { get; set; }
        public string CourseName { get; set; }
        public int CourseLevel { get; set; }
        public string? CourseOption { get; set; }
        public int DurationInMonths { get; set; }
        public DateTime PlannedStartDate { get; set; }
        public DateTime PlannedEndDate { get; set; }
    }
}