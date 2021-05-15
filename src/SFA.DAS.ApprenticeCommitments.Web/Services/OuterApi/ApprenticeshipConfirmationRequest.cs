namespace SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi
{
    public class ApprenticeshipConfirmationRequest
    {
        public ApprenticeshipConfirmationRequest(bool apprenticeshipCorrect)
        {
            ApprenticeshipCorrect = apprenticeshipCorrect;
        }

        public bool ApprenticeshipCorrect { get; }
    }
}