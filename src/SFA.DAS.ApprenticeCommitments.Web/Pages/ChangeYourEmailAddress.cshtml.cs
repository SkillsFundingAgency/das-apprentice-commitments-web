using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SFA.DAS.ApprenticeCommitments.Web.Startup;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages
{
    public class ChangeYourEmailAddressModel : PageModel
    {
        private readonly AuthenticationServiceConfiguration authenticationConfig;

        public ChangeYourEmailAddressModel(AuthenticationServiceConfiguration authConfig)
            => authenticationConfig = authConfig;

        public IActionResult OnGet() => Redirect(
            $"{authenticationConfig.MetadataAddress}{authenticationConfig.ChangeEmailPath}");
    }
}
