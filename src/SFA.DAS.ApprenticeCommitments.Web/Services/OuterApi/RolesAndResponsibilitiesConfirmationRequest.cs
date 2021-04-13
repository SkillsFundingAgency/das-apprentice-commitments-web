namespace SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi
{
    public class RolesAndResponsibilitiesConfirmationRequest
    {
        public RolesAndResponsibilitiesConfirmationRequest(bool rolesAndResponsibilitiesCorrect)
            => RolesAndResponsibilitiesCorrect = rolesAndResponsibilitiesCorrect;

        public bool RolesAndResponsibilitiesCorrect { get; }
    }
}
