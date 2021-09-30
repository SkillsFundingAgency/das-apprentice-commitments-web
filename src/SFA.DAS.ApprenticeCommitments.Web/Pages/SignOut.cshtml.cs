using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;

using SFA.DAS.ApprenticeCommitments.Web.Services;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages
{
    public class SignOutModel : PageModel
    {
        private readonly AuthenticatedUser _authenticatedUser;

        public SignOutModel(AuthenticatedUser authenticatedUser)
        {
            _authenticatedUser = authenticatedUser;
        }

        public IActionResult OnGet()
        {
            // clearup cookies
            if (HttpContext.Request.Cookies.Count > 0)
            {
                var cookies = HttpContext.Request.Cookies.Where(x => x.Key.Contains("Apprenticeships") || x.Key.Contains("AspNetCore")).ToList();

                cookies.ForEach(x => Response.Cookies.Delete(x.Key));
            }

            var result = SignOut(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            OpenIdConnectDefaults.AuthenticationScheme);

            return result;
        }
    }
}