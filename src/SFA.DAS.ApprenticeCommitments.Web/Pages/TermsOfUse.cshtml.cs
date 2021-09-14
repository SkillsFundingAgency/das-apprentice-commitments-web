using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using SFA.DAS.ApprenticePortal.SharedUi.Menu;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages
{

    public class TermsOfUseModel : PageModel
    {
        private readonly AuthenticatedUser apprentice;
        private readonly ApprenticeApi _client;
        private readonly NavigationUrlHelper _urlHelper;

        [BindProperty]
        public bool TermsOfUseAccepted { get; set; }

        public TermsOfUseModel(AuthenticatedUser _apprentice, ApprenticeApi client, NavigationUrlHelper urlHelper)
        {
            apprentice = _apprentice;
            _client = client;
            _urlHelper = urlHelper;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            if (TermsOfUseAccepted)
            {
                await _client.AcceptTermsOfUse(apprentice.ApprenticeId);
                await AuthenticationEvents.TermsOfUseAccepted(HttpContext);
            }

            if (Request.Cookies.Keys.Contains("RegistrationCode"))
                return RedirectToAction("Register", "Registration");
            else
                return Redirect(_urlHelper.Generate(NavigationSection.Home, "Home"));

        }
    }
}