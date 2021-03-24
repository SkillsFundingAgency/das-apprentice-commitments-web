namespace SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi
{
    public class EmployerConfirmationRequest
    {
        public EmployerConfirmationRequest(bool employerCorrect)
        {
            EmployerCorrect = employerCorrect;
        }

        public bool EmployerCorrect { get; }
    }
}