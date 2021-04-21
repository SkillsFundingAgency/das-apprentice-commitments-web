using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SFA.DAS.ApprenticeCommitments.Web.Identity;
using SFA.DAS.ApprenticeCommitments.Web.Services;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships
{
    public class TransactionCompleteModel : PageModel, IHasBackLink
    {
        private readonly AuthenticatedUserClient _client;

        [BindProperty(SupportsGet = true)]
        public HashedId ApprenticeshipId { get; set; }

        [BindProperty]
        public string CourseName { get; set; }

        public string Backlink => $"/apprenticeships/{ApprenticeshipId.Hashed}";

        public TransactionCompleteModel(AuthenticatedUserClient client)
        {
            _client = client;
        }

        public async Task OnGet()
        {
            var apprenticeship = await _client.GetApprenticeship(ApprenticeshipId.Id);
            CourseName = apprenticeship.CourseName;
        }
    }
}
