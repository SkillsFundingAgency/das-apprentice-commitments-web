using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SFA.DAS.ApprenticeCommitments.Web.Pages.IdentityHashing;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;

#nullable enable

namespace SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships
{
    public class ConfirmYourEmployerModel : PageModel, IHasBackLink
    {
        private readonly IOuterApiClient _client;
        private readonly AuthenticatedUser _authenticatedUser;

        [BindProperty(SupportsGet = true)]
        public HashedId ApprenticeshipId { get; set; }
        [BindProperty]
        public string EmployerName { get; set; }
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
                .GetApprenticeship(_authenticatedUser.RegistrationId, ApprenticeshipId.Id);
            EmployerName = apprenticeship.EmployerName;
            ConfirmedEmployer = apprenticeship.EmployerCorrect;
        }

        public async Task<IActionResult> OnPost()
        {

            if (ConfirmedEmployer == null)
            {
                ModelState.AddModelError(nameof(ConfirmedEmployer), "Select an answer");
                return new PageResult();
            }

            await _client.ConfirmEmployer(
                _authenticatedUser.RegistrationId, ApprenticeshipId.Id,
                new EmployerConfirmationRequest(ConfirmedEmployer.Value));

            var nextPage = ConfirmedEmployer.Value ? "Confirm" : "CannotConfirm";

            return new RedirectToPageResult(nextPage, new { ApprenticeshipId });
        }
    }
}
