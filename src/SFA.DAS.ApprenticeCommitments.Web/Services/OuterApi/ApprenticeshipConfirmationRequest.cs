namespace SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi
{
    public class ApprenticeshipConfirmationRequest
    {
        public ApprenticeshipConfirmationRequest(bool apprenticeshipCorrect)
        {
            ApprenticeshipCorrect = apprenticeshipCorrect;
        }

        public ApprenticeshipConfirmationRequest()
        {
        }

        public static ApprenticeshipConfirmationRequest ConfirmTrainingProvider(bool correct)
            => new ApprenticeshipConfirmationRequest { TrainingProviderCorrect = correct };

        public static ApprenticeshipConfirmationRequest ConfirmEmployer(bool correct)
            => new ApprenticeshipConfirmationRequest { EmployerCorrect = correct };

        public static ApprenticeshipConfirmationRequest ConfirmDelivery(bool correct)
            => new ApprenticeshipConfirmationRequest { HowApprenticeshipDeliveredCorrect = correct };

        public static ApprenticeshipConfirmationRequest ConfirmApprenticeshipDetails(bool correct)
            => new ApprenticeshipConfirmationRequest { ApprenticeshipDetailsCorrect = correct };

        public static ApprenticeshipConfirmationRequest ConfirmRolesAndResponsibilities(bool correct)
            => new ApprenticeshipConfirmationRequest { RolesAndResponsibilitiesCorrect = correct };

        public bool? EmployerCorrect { get; set; }
        public bool? TrainingProviderCorrect { get; set; }
        public bool? ApprenticeshipDetailsCorrect { get; set; }
        public bool? RolesAndResponsibilitiesCorrect { get; set; }
        public bool? HowApprenticeshipDeliveredCorrect { get; set; }
        public bool? ApprenticeshipCorrect { get; set; }
    }
}