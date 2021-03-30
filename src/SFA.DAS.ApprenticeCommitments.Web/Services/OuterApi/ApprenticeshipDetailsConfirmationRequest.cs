namespace SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi
{
    public class ApprenticeshipDetailsConfirmationRequest
    {
        public ApprenticeshipDetailsConfirmationRequest(bool apprenticeshipDetailsCorrect)
        {
            ApprenticeshipDetailsCorrect = apprenticeshipDetailsCorrect;
        }

        public bool ApprenticeshipDetailsCorrect { get; }
    }
}
