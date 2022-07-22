using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using SFA.DAS.ApprenticePortal.SharedUi.Menu;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SFA.DAS.ApprenticePortal.Authentication;
using SFA.DAS.ApprenticeCommitments.Web.Helpers;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.ApprenticeCommitments.Web.Controllers
{
    [AllowAnonymous]
    public class RegistrationControllerInitial : Controller
    {
        private readonly DomainHelper _domainHelper;
        private readonly ILogger<RegistrationControllerInitial> _logger;

        public RegistrationControllerInitial(DomainHelper domainHelper, ILogger<RegistrationControllerInitial> logger)
        {
            _domainHelper = domainHelper;
            _logger = logger;
        }

        [HttpGet("/register/{registrationCode}")]
        public IActionResult Register(string registrationCode)
        {
            _logger.LogInformation("Starting registration of {RegistrationId}", registrationCode);
            Response.Cookies.Append("RegistrationCode", registrationCode, new CookieOptions
            {
                Domain = _domainHelper.ParentDomain, 
                Secure = _domainHelper.Secure,
                HttpOnly = true
            });
            return RedirectToAction("Register", "Registration");
        }
    }

    [Authorize]
    public class RegistrationController : Controller
    {
        private readonly AuthenticatedUser _user;
        private readonly ApprenticeApi _registrations;
        private readonly NavigationUrlHelper _urlHelper;
        private readonly DomainHelper _domainHelper;
        private readonly ILogger<RegistrationController> _logger;

        public RegistrationController(AuthenticatedUser user, ApprenticeApi registrations, NavigationUrlHelper urlHelper, DomainHelper domainHelper, ILogger<RegistrationController> logger)
        {
            _user = user;
            _registrations = registrations;
            _urlHelper = urlHelper;
            _domainHelper = domainHelper;
            _logger = logger;
        }

        [HttpGet("/register")]
        public async Task<IActionResult> Register()
        {
            if (!_user.HasCreatedAccount)
                return Redirect(_urlHelper.Generate(NavigationSection.PersonalDetails));

            if (!Request.Cookies.TryGetValue("RegistrationCode", out var registrationCode))
                return RedirectToHome();

            try
            {
                _logger.LogInformation("Starting registration of {RegistrationId} to apprentice {ApprenticeId}", registrationCode, _user.ApprenticeId);

                await _registrations.MatchApprenticeToApprenticeship(registrationCode!, _user.ApprenticeId);
                Response.Cookies.Delete("RegistrationCode", new CookieOptions
                {
                    Domain = _domainHelper.ParentDomain
                });
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
