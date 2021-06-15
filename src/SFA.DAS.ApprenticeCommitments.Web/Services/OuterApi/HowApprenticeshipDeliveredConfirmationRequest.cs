namespace SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi
{
    public class HowApprenticeshipDeliveredConfirmationRequest
    {
        public HowApprenticeshipDeliveredConfirmationRequest(bool howApprenticeshipDeliveredCorrect)
        {
            HowApprenticeshipDeliveredCorrect = howApprenticeshipDeliveredCorrect;
        }

        public bool HowApprenticeshipDeliveredCorrect { get; }
    }
}