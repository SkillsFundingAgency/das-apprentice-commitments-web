using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using SFA.DAS.ApprenticeCommitments.Web.Pages.IdentityHashing;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships
{
    public class ConfirmYourTrainingModel : PageModel, IHasBackLink
    {
        private readonly IOuterApiClient _client;
        private readonly AuthenticatedUser _authenticatedUser;

        [BindProperty(SupportsGet = true)]
        public HashedId ApprenticeshipId { get; set; }

        [BindProperty]
        public string TrainingProviderName { get; set; }

        [BindProperty]
        public bool? ConfirmTrainingProvider { get; set; }

        public string Backlink => $"/apprenticeships/{ApprenticeshipId.Hashed}";

        public ConfirmYourTrainingModel(IOuterApiClient client, AuthenticatedUser authenticatedUser)
        {
            _authenticatedUser = authenticatedUser;
            _client = client;
        }

        public async Task OnGetAsync()
        {
            var apprenticeship = await _client
                .GetApprenticeship(_authenticatedUser.RegistrationId, ApprenticeshipId.Id);
            TrainingProviderName = apprenticeship.TrainingProviderName;
        }

        public IActionResult OnPost()
        {
            switch (ConfirmTrainingProvider)
            {
                case null:
                    ModelState.AddModelError(nameof(ConfirmTrainingProvider), "Select an answer");
                    return new PageResult();

                case true:
                    return new RedirectToPageResult("Confirm", new { ApprenticeshipId });

                default:
                    return new RedirectToPageResult("CannotConfirm", new { ApprenticeshipId });
            }
        }
    }
}