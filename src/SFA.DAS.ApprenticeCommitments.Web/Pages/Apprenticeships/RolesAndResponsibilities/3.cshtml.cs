using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;
using SFA.DAS.ApprenticePortal.SharedUi.Menu;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships.RolesAndResponsibilities
{
    [HideNavigationBar]
    public class RolesAndResponsibilitiesForProviderModel : SectionConfirmationPageModel
    {
        private readonly RolesAndResponsibilitiesConfirmations _rolesAndResponsibilitiesConfirmations =
            RolesAndResponsibilitiesConfirmations.ProviderRolesAndResponsibilitiesConfirmed;
        public RolesAndResponsibilitiesForProviderModel(AuthenticatedUserClient client) : base(client, 3)
        {}

        public Task<IActionResult> OnGet()
        {
            return GetConfirmationSection(_rolesAndResponsibilitiesConfirmations);
        }

        public Task<IActionResult> OnPost()
        {
            return SaveConfirmationStatus(_rolesAndResponsibilitiesConfirmations);
        }
    }
}