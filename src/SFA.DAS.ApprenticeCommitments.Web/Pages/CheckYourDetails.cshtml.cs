using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using SFA.DAS.ApprenticePortal.Authentication;
using SFA.DAS.ApprenticePortal.SharedUi.Filters;
using SFA.DAS.ApprenticePortal.SharedUi.Menu;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages
{
    [HideNavigationBar]
    [RequiresIdentityConfirmed]
    public class CheckYourDetails : PageModel
    {
        private readonly ApprenticeApi _apprentices;
        private readonly NavigationUrlHelper _urlHelper;

        public CheckYourDetails(ApprenticeApi _apprentices, NavigationUrlHelper urlHelper)
        {
            this._apprentices = _apprentices;
            _urlHelper = urlHelper;
        }

        public string FirstName { get; private set; } = null!;
        public string LastName { get; private set; } = null!;
        public DateTime? DateOfBirth { get; private set; }

        public async Task<ActionResult> OnGet(
            [FromServices] AuthenticatedUser user)
        {
            var apprentice = await _apprentices.TryGetApprentice(user.ApprenticeId);
            if (apprentice == null) return Redirect(_urlHelper.Generate(NavigationSection.ApprenticeAccounts, $"Account"));

            FirstName = apprentice.FirstName;
            LastName = apprentice.LastName;
            DateOfBirth = apprentice.DateOfBirth;

            return Page();
        }
    }
}
