using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using SFA.DAS.ApprenticePortal.SharedUi.Menu;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.Controllers
{
    [AllowAnonymous]
    public class RegistrationControllerInitial : Controller
    {
        [HttpGet("/register/{registrationCode}")]
        public IActionResult Register(string registrationCode)
        {
            Response.Cookies.Append("RegistrationCode", registrationCode);
            return RedirectToAction("Register", "Registration");
        }
    }

    [Authorize]
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
        public async Task<IActionResult> Register()
        {
            if (UserAccountCreatedClaim.UserMustCreateAccount(HttpContext))
                return Redirect(_urlHelper.Generate(NavigationSection.PersonalDetails));

            if (!Request.Cookies.TryGetValue("RegistrationCode", out var registrationCode))
                return RedirectToHome();

            try
            {
                await _registrations.MatchApprenticeToApprenticeship(registrationCode, _user.ApprenticeId);
                Response.Cookies.Delete("RegistrationCode");
                return RedirectToNotice("ApprenticeshipMatched");
            }
            catch
            {
                return RedirectToPage("/CheckYourDetails");
            }
        }

        private RedirectResult RedirectToHome()
            => Redirect(_urlHelper.Generate(NavigationSection.Home, "Home"));

        private RedirectResult RedirectToNotice(string notification)
            => Redirect(_urlHelper.Generate(NavigationSection.Home, $"Home?notification={notification}"));
    }
}