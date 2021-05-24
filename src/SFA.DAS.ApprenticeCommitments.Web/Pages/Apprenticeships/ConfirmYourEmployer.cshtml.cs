using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SFA.DAS.ApprenticeCommitments.Web.Identity;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships
{
    [RequiresIdentityConfirmed]
    public class ConfirmYourEmployerModel : PageModel, IHasBackLink
    {
        private readonly IOuterApiClient _client;
        private readonly AuthenticatedUser _authenticatedUser;

        [BindProperty(SupportsGet = true)]
        public HashedId ApprenticeshipId { get; set; }

        [BindProperty]
        public string EmployerName { get; set; } = null!;

        [BindProperty]
        public bool? ConfirmedEmployer { get; set; }

        public string Backlink => $"/apprenticeships/{ApprenticeshipId.Hashed}";

        public ConfirmYourEmployerModel(IOuterApiClient client, AuthenticatedUser authenticatedUser)
        {
            _client = client;
            _authenticatedUser = authenticatedUser;
        }

        public async Task OnGet()
        {
            var apprenticeship = await _client
                .GetApprenticeship(_authenticatedUser.ApprenticeId, ApprenticeshipId.Id);

            EmployerName = apprenticeship.EmployerName;

            if (apprenticeship.EmployerCorrect == true)
                ConfirmedEmployer = true;
        }

        public async Task<IActionResult> OnPost()
        {
            if (ConfirmedEmployer == null)
            {
                ModelState.AddModelError(nameof(ConfirmedEmployer), "Select an answer");
                return new PageResult();
            }

            await _client.ConfirmEmployer(
                _authenticatedUser.ApprenticeId, ApprenticeshipId.Id,
                new EmployerConfirmationRequest(ConfirmedEmployer.Value));

            var nextPage = ConfirmedEmployer.Value ? "Confirm" : "CannotConfirm";

            return new RedirectToPageResult(nextPage, new { ApprenticeshipId });
        }
    }
}