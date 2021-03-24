namespace SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi
{
    public class TrainingProviderConfirmationRequest
    {
        public TrainingProviderConfirmationRequest(bool trainingProviderCorrect)
            => TrainingProviderCorrect = trainingProviderCorrect;

        public bool TrainingProviderCorrect { get; }
    }
}