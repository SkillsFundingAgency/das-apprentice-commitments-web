using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;
using SFA.DAS.ApprenticePortal.SharedUi.Filters;
using SFA.DAS.ApprenticePortal.SharedUi.Menu;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships
{
    [RequiresIdentityConfirmed]
    [HideNavigationBar]
    public class ConfirmYourEmployerModel : ApprenticeshipConfirmationPageModel
    {
        [BindProperty]
        public string EmployerName { get; set; } = null!;

        public ConfirmYourEmployerModel(AuthenticatedUserClient client) : base("Employer", client)
        {
        }

        public override void LoadApprenticeship(Apprenticeship apprenticeship)
        {
            EmployerName = apprenticeship.EmployerName;
            Confirmed = apprenticeship.EmployerCorrect;
        }

        public override ApprenticeshipConfirmationRequest CreateUpdate(bool confirmed)
            => ApprenticeshipConfirmationRequest.ConfirmEmployer(confirmed);
    }
}