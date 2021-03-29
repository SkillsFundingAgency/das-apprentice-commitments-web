namespace SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi
{
    public class ApprenticeshipDetailsRequest
    {
        public ApprenticeshipDetailsRequest(bool apprenticeshipDetailsCorrect)
        {
            ApprenticeshipDetailsCorrect = apprenticeshipDetailsCorrect;
        }

        public bool ApprenticeshipDetailsCorrect { get; }
    }
}
