using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using SFA.DAS.ApprenticePortal.SharedUi.Menu;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly AuthenticatedUser _user;
        private readonly ApprenticeApi _registrations;
        private readonly NavigationUrlHelper _urlHelper;

        public RegistrationController(AuthenticatedUser user, ApprenticeApi registrations, NavigationUrlHelper urlHepler)
        {
            _user = user;
            _registrations = registrations;
            _urlHelper = urlHepler;
        }

        [HttpGet("/register")]
        public async Task<IActionResult> Register([FromQuery] string registrationCode)
        {
            if (string.IsNullOrEmpty(registrationCode)) return RedirectToHome();

            await _registrations.RegistrationSeen(registrationCode, DateTime.UtcNow);
            Response.Cookies.Append("RegistrationCode", registrationCode);

            if (UserAccountCreatedClaim.UserHasNotCreatedAccount(HttpContext))
                return RedirectToPage("/Account", "register");

            try
            {
                await _registrations.MatchApprenticeToApprenticeship(registrationCode, _user.ApprenticeId);
                Response.Cookies.Delete("RegistrationCode");
                return RedirectToNotice("ApprenticeshipMatched");
            }
            catch
            {
                return RedirectToNotice("ApprenticeshipDidNotMatch");
            }
        }

        private RedirectResult RedirectToHome()
            => Redirect(_urlHelper.Generate(NavigationSection.Home, "Home"));

        private RedirectResult RedirectToNotice(string notification)
            => Redirect(_urlHelper.Generate(NavigationSection.Home, $"Home?notification={notification}"));
    }
}