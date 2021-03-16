using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;
using SFA.DAS.HashingService;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships
{
    public class ConfirmYourEmployerModel : PageModel, IHasBackLink
    {
        private readonly IHashingService _hashingService;
        private readonly IOuterApiClient _client;
        private readonly AuthenticatedUser _authenticatedUser;

        [BindProperty(SupportsGet = true)]
        public string ApprenticeshipId { get; set; }
        [BindProperty]
        public string EmployerName { get; set; }
        [BindProperty]
        public bool? EmployerConfirm { get; set; }
        public string Backlink => $"/apprenticeships/{ApprenticeshipId}";

        public ConfirmYourEmployerModel(IHashingService hashingService, IOuterApiClient client, AuthenticatedUser authenticatedUser)
        {
            _hashingService = hashingService;
            _client = client;
            _authenticatedUser = authenticatedUser;
        }

        public async Task OnGet()
        {
            var apprenticeshipId = _hashingService.DecodeValue(ApprenticeshipId);
            var apprenticeship = await _client.GetApprenticeship(_authenticatedUser.RegistrationId, apprenticeshipId);
            EmployerName = apprenticeship.EmployerName;
        }

        public IActionResult OnPost()
        {
            switch (EmployerConfirm)
            {
                case null:
                    ModelState.AddModelError(nameof(EmployerConfirm), "Select an answer");
                    return new PageResult();
                case true:
                    return new RedirectToPageResult("Confirm", new { ApprenticeshipId });
                default:
                    return new RedirectToPageResult("CannotConfirm", new { ApprenticeshipId });
            }
        }
    }
}
