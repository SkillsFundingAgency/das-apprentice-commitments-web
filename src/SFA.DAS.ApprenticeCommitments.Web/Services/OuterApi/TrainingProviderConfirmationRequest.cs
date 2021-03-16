namespace SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi
{
    public class TrainingProviderConfirmationRequest
    {
        public TrainingProviderConfirmationRequest(bool confirmedTrainingProvider)
            => ConfirmedTrainingProvider = confirmedTrainingProvider;

        public bool ConfirmedTrainingProvider { get; }
    }
}