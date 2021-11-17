using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;
using SFA.DAS.ApprenticePortal.SharedUi.Menu;
using StackExchange.Redis;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships.RolesAndResponsibilities
{
    [HideNavigationBar]
    public class RolesAndResponsibilitiesForProviderModel : SectionConfirmationPageModel
    {
        private RolesAndResponsibilitiesConfirmations _rolesAndResponsibilitiesConfirmations =
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