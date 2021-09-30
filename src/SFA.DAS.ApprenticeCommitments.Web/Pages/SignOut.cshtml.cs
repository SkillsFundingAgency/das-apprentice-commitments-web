using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages
{
    public class SignOutModel : PageModel
    {
        public IActionResult OnGet()
        {
            var result = SignOut(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            OpenIdConnectDefaults.AuthenticationScheme);

            // clearup cookies
            if (HttpContext.Request.Cookies.Count > 0)
            {
                var cookies = HttpContext.Request.Cookies.Where(x => x.Key.Contains("AspNetCore") || x.Key.Contains("Apprenticeships")).ToList();

                cookies.ForEach(x => Response.Cookies.Delete(x.Key));
            }

            return result;
        }
    }
}