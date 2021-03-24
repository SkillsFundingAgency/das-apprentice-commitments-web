using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SFA.DAS.ApprenticeCommitments.Web.Pages.IdentityHashing;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships
{
    public class YourApprenticeshipDetails : PageModel, IHasBackLink
    {
        private readonly IOuterApiClient _client;
        private readonly AuthenticatedUser _authenticatedUser;

        [BindProperty(SupportsGet = true)]
        public HashedId ApprenticeshipId { get; set; }

        [BindProperty]
        public bool? ComfirmApprenticeshipDetails { get; set; }
        public string Backlink => $"/apprenticeships/{ApprenticeshipId.Hashed}";

        public YourApprenticeshipDetails(IOuterApiClient client, AuthenticatedUser authenticatedUser)
        {
            _client = client;
            _authenticatedUser = authenticatedUser;
        }

        public IActionResult OnPost()
        {
            switch (ComfirmApprenticeshipDetails)
            {
                case null:
                    ModelState.AddModelError(nameof(ComfirmApprenticeshipDetails), "Select an answer");
                    return new PageResult();
                case true:
                    return new RedirectToPageResult("Confirm", new { ApprenticeshipId });
                default:
                    return new RedirectToPageResult("CannotConfirm", new { ApprenticeshipId });
            }
        }
    }
}
