using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using SFA.DAS.ApprenticePortal.SharedUi.Menu;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly AuthenticatedUser _user;
        private readonly RegistrationsService _registrations;
        private readonly NavigationUrlHelper _urlHelper;

        public RegistrationController(AuthenticatedUser user, RegistrationsService registrations, NavigationUrlHelper urlHepler)
        {
            _user = user;
            _registrations = registrations;
            _urlHelper = urlHepler;
        }

        [HttpGet("/register")]
        public async Task<IActionResult> Register([FromQuery] string registrationCode)
        {
            if (VerifiedUser.UserHasUnconfirmedIdentity(HttpContext))
            {
                var routeValuesDictionary = new RouteValueDictionary();
                foreach (var a in Request.Query) routeValuesDictionary.Add(a.Key, a.Value);
                return RedirectToPage("/Account", "register", routeValuesDictionary);
            }

            try
            {
                await _registrations.MatchApprenticeToApprenticeship(registrationCode, _user.ApprenticeId);
                return RedirectToNotice("ApprenticeshipMatched");
            }
            catch
            {
                return RedirectToNotice("ApprenticeshipDidNotMatch");
            }
        }

        private RedirectResult RedirectToNotice(string notification)
        {
            return Redirect(_urlHelper.Generate(NavigationSection.Home, $"?notification={notification}"));
        }
    }
}