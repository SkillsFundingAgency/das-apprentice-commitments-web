namespace SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi
{
    public static class ApprenticeshipExtensions
    {
        public static bool IsCompleted(this Apprenticeship apprenticeship) =>
            apprenticeship.EmployerCorrect == true &&
            apprenticeship.TrainingProviderCorrect == true &&
            apprenticeship.ApprenticeshipDetailsCorrect == true &&
            apprenticeship.RolesAndResponsibilitiesCorrect == true &&
            apprenticeship.HowApprenticeshipDeliveredCorrect == true;
    }
}