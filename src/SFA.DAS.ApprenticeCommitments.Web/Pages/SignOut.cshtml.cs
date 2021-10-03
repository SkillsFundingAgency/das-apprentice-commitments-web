using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using System.Web;
using Microsoft.AspNetCore.Http;


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
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }

            var result = SignOut(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            OpenIdConnectDefaults.AuthenticationScheme);

            //HttpContext.SignOutAsync().Wait();

            //// clearup cookies
            //if (HttpContext.Request.Cookies.Count > 0)
            //{
            //    var cookies = HttpContext.Request.Cookies.Where(x => x.Key.Contains("Apprenticeships") || x.Key.Contains("AspNetCore")).ToList();

            //    //cookies.ForEach(x => Response.Cookies.Delete(x.Key));
            //    cookies.ForEach(x =>
            //    {
            //        var options = new CookieOptions { Expires = System.DateTime.Now.AddDays(-1) };
            //        this.Response.Cookies.Append(x.Key, x.Value, options);
            //    });
            //}

            return result;
        }

        public void OnPost()
        {
            // clearup cookies
            if (HttpContext.Request.Cookies.Count > 0)
            {
                var cookies = HttpContext.Request.Cookies.Where(x => x.Key.Contains("Apprenticeships") || x.Key.Contains("AspNetCore")).ToList();

                //cookies.ForEach(x => Response.Cookies.Delete(x.Key));
                cookies.ForEach(x =>
                {
                    var options = new CookieOptions { Expires = System.DateTime.Now.AddDays(-1) };
                    this.Response.Cookies.Append(x.Key, x.Value, options);
                });
            }
        }
    }
}