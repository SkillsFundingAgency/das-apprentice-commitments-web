using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages
{
    public class SignOutModel : PageModel
    {
        public IActionResult OnGet()
        {
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }

            return SignOut(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            OpenIdConnectDefaults.AuthenticationScheme);
        }
    }
}