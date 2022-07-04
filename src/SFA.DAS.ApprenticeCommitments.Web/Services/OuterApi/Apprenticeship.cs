using System;
using System.ComponentModel.DataAnnotations;

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
        public RolesAndResponsibilitiesConfirmations RolesAndResponsibilitiesConfirmations { get; set; }
        public bool? ApprenticeshipDetailsCorrect { get; set; }
        public bool? HowApprenticeshipDeliveredCorrect { get; set; }
        public DateTime ConfirmBefore { get; set; }
        public DateTime? ConfirmedOn { get; set; }
        public DateTime? LastViewed { get; set; }
        public DeliveryModel DeliveryModel { get; set; }
        public string CourseName { get; set; } = null!;
        public int CourseLevel { get; set; }
        public string? CourseOption { get; set; } = null!;
        public int CourseDuration { get; set; }
        public int DurationInMonths { get; set; }
        public DateTime PlannedStartDate { get; set; }
        public DateTime PlannedEndDate { get; set; }
        public DateTime? EmploymentEndDate { get; set; }
        public DateTime? StoppedReceivedOn { get; set; }
        public bool IsStopped { get; set; }
        public ChangeOfCircumstanceNotifications ChangeOfCircumstanceNotifications { get; set; }
    }

    public enum DeliveryModel
    {
        [Display(Name = "Regular")]
        Regular = 0,
        
        [Display(Name = "Portable flexi-job")]
        PortableFlexiJob = 1,

        [Display(Name = "Flexi-job Agency")]
        FlexiJobAgency = 2,
    }

    [Flags]
    public enum ChangeOfCircumstanceNotifications
    {
        None = 0,
        EmployerDetailsChanged = 1,
        ProviderDetailsChanged = 2,
        ApprenticeshipDetailsChanged = 4,
    }

    [Flags]
    public enum RolesAndResponsibilitiesConfirmations
    {
        None = 0,
        ApprenticeRolesAndResponsibilitiesConfirmed = 1,
        EmployerRolesAndResponsibilitiesConfirmed = 2,
        ProviderRolesAndResponsibilitiesConfirmed = 4
    }

    public static class RolesAndResponsibilitiesConfirmationsExtensions
    {
        public static bool IsConfirmed(this RolesAndResponsibilitiesConfirmations confirmations)
        {
            if (confirmations.HasFlag(RolesAndResponsibilitiesConfirmations.ApprenticeRolesAndResponsibilitiesConfirmed) &&
                confirmations.HasFlag(RolesAndResponsibilitiesConfirmations.EmployerRolesAndResponsibilitiesConfirmed) &&
                confirmations.HasFlag(RolesAndResponsibilitiesConfirmations.ProviderRolesAndResponsibilitiesConfirmed))
            {
                return true;
            }

            return false;
        }
    }
}