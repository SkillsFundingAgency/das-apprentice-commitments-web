using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;
using SFA.DAS.ApprenticePortal.SharedUi.Menu;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships.RolesAndResponsibilities
{
    [HideNavigationBar]
    public class RolesAndResponsibilitiesModel : ApprenticeshipRevisionPageModel
    {
        private readonly AuthenticatedUserClient _client;

        [BindProperty]
        public bool? RolesAndResponsibilitiesConfirmed { get; set; } = null!;

        public RolesAndResponsibilitiesModel(AuthenticatedUserClient client)
        {
            _client = client;
        }

        public async Task<IActionResult> OnGet()
        {
            var apprenticeship = await OnGetAsync(_client);

            if (!apprenticeship.RolesAndResponsibilitiesConfirmations.IsConfirmed())
            {
                return new RedirectToPageResult("1", new { ApprenticeshipId });
            }

            return Page();
        }
    }
}