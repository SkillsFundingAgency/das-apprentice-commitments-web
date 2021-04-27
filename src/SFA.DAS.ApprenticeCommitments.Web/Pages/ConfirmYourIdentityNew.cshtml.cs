using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages
{
    public class ConfirmYourIdentityNewModel : PageModel
    {
        public async Task<IActionResult> OnGet()
        {
            // This page is called from the login service link once a new user has entetred a password
            // It ensures that any already loggin in user is first logged out
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);

            return RedirectToPage("ConfirmYourIdentity");
        }
    }
}
