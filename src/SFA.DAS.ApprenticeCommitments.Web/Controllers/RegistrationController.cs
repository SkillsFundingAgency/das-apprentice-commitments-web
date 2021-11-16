using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using SFA.DAS.ApprenticePortal.SharedUi.Menu;
using System;
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
        private readonly ILogger<RegistrationController> _logger;

        public RegistrationController(
            AuthenticatedUser user,
            ApprenticeApi registrations,
            NavigationUrlHelper urlHepler,
            ILogger<RegistrationController> logger)
        {
            _user = user;
            _registrations = registrations;
            _urlHelper = urlHepler;
            _logger = logger;
        }

        [HttpGet("/register")]
        public async Task<IActionResult> Register()
        {
            try
            {
                _logger.LogInformation("Register apprentice {apprenticeId}", _user.ApprenticeId);

                if (UserAccountCreatedClaim.UserMustCreateAccount(HttpContext))
                    return RedirectToPage("/Account", "register");

                if (!Request.Cookies.TryGetValue("RegistrationCode", out var registrationCode))
                    return RedirectToHome();

                _logger.LogInformation("Register apprentice {apprenticeId} to registration {registrationId}", _user.ApprenticeId, registrationCode);

                await _registrations.MatchApprenticeToApprenticeship(registrationCode, _user.ApprenticeId);
                Response.Cookies.Delete("RegistrationCode");
                return RedirectToNotice("ApprenticeshipMatched");
            }
            catch(Exception e)
            {
                _logger.LogInformation(e, "Register apprentice error {apprenticeId}", _user.ApprenticeId);
                return RedirectToPage("/CheckYourDetails");
            }
        }

        private RedirectResult RedirectToHome()
            => Redirect(_urlHelper.Generate(NavigationSection.Home, "Home"));

        private RedirectResult RedirectToNotice(string notification)
            => Redirect(_urlHelper.Generate(NavigationSection.Home, $"Home?notification={notification}"));
    }
}