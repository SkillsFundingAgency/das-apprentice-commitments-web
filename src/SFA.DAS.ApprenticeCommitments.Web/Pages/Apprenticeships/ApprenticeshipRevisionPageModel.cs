using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SFA.DAS.ApprenticeCommitments.Web.Identity;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships
{
    public class ApprenticeshipRevisionPageModel : PageModel, IHasBackLink, IHaveDeliveryModel
    {
        [BindProperty(SupportsGet = true)]
        public HashedId ApprenticeshipId { get; set; }

        [BindProperty]
        public long RevisionId { get; set; }

        public DeliveryModel DeliveryModel { get; set; }

        public virtual string Backlink => $"/apprenticeships/{ApprenticeshipId.Hashed}";

        protected async Task<Apprenticeship> OnGetAsync(AuthenticatedUserClient _client)
        {
            var apprenticeship = await _client.GetApprenticeship(ApprenticeshipId.Id);
            RevisionId = apprenticeship.RevisionId;
            DeliveryModel = apprenticeship.DeliveryModel;
            return apprenticeship;
        }
    }
}