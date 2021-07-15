using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages
{
    public class ConfirmYourPersonalDetailsNewModel : PageModel
    {
        public async Task<IActionResult> OnGet()
        {
            // This page is called from the login service link once a new user has entetred a password
            // It ensures that any already logged in user is first logged out
            if (User?.Identity.IsAuthenticated == true)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);

                return new EmptyResult();
            }
            else
            {
                return RedirectToPage("ConfirmYourPersonalDetails");
            }
        }
    }
}