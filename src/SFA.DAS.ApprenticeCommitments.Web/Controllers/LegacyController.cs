using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticePortal.SharedUi.Menu;

namespace SFA.DAS.ApprenticeCommitments.Web.Controllers
{
    [AllowAnonymous]
    public class LegacyController : Controller
    {
        private readonly NavigationUrlHelper _urlHelper;

        public LegacyController(NavigationUrlHelper urlHelper )
        {
            _urlHelper = urlHelper;
        }

        [HttpGet("/account")]
        public IActionResult Account(string registrationCode)
            => Redirect(_urlHelper.Generate(NavigationSection.PersonalDetails));

        [HttpGet("/termsofuse")]
        public IActionResult Register(string registrationCode)
            => Redirect(_urlHelper.Generate(NavigationSection.TermsOfUse));
    }
}