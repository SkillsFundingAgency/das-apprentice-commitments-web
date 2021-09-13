using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SFA.DAS.ApprenticePortal.SharedUi.Menu;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages
{

    public class TermsOfUseModel : PageModel
    {
        private readonly NavigationUrlHelper _urlHelper;

        public TermsOfUseModel(NavigationUrlHelper urlHelper) => _urlHelper = urlHelper;

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (Request.Cookies.Keys.Contains("RegistrationCode"))
                return RedirectToAction("Register", "Registration");
            else
                return Redirect(_urlHelper.Generate(NavigationSection.Home, "Home"));

        }
    }
}