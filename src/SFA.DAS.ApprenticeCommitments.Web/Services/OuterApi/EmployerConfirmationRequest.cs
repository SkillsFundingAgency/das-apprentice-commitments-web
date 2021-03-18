namespace SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi
{
    public class EmployerConfirmationRequest
    {
        public EmployerConfirmationRequest(bool confirmedEmployer)
        {
            ConfirmedEmployer = confirmedEmployer;
        }

        public bool ConfirmedEmployer { get; }
    }
}