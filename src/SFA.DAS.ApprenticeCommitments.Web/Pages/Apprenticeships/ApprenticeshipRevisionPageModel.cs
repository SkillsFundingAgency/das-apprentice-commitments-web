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
        public bool ExplicitRevision { get; set; }

        public virtual string Backlink
        {
            get
            {
                if(ExplicitRevision)
                    return $"/apprenticeships/{ApprenticeshipId.Hashed}/view";
                return $"/apprenticeships/{ApprenticeshipId.Hashed}";
            }
        }

        protected async Task<Apprenticeship> OnGetAsync(AuthenticatedUserClient _client, long? revisionId = null)
        {
            ExplicitRevision = revisionId.HasValue;
            var apprenticeship = revisionId == null ? await _client.GetApprenticeship(ApprenticeshipId.Id) : await _client.GetApprenticeshipRevision(ApprenticeshipId.Id, revisionId.Value);
            RevisionId = apprenticeship.RevisionId;
            DeliveryModel = apprenticeship.DeliveryModel;
            return apprenticeship;
        }
    }
}