using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships.RolesAndResponsibilities
{
    public class SectionConfirmationPageModel : ApprenticeshipRevisionPageModel
    {
        protected readonly AuthenticatedUserClient Client;
        protected readonly byte SectionPage;

        protected const string ConfirmationErrorMessage = "Please confirm that you have read the roles and responsibilities";

        [BindProperty]
        public bool SectionConfirmed { get; set; }

        public override string Backlink
        {
            get
            {
                if (SectionPage == 1)
                    return $"/apprenticeships/{ApprenticeshipId.Hashed}";
                return $"/apprenticeships/{ApprenticeshipId.Hashed}/rolesandresponsibilities/{SectionPage - 1}";
            }
        }

        public string NextPage
        {
            get
            {
                if (SectionPage == 3)
                    return $"/Apprenticeships/Index";
                return $"{SectionPage + 1}";
            }
        }

        public SectionConfirmationPageModel(AuthenticatedUserClient client, byte sectionPage)
        {
            Client = client;
            SectionPage = sectionPage;
        }

        protected async Task<IActionResult> GetConfirmationSection(RolesAndResponsibilitiesConfirmations rolesAndResponsibilitiesConfirmation)
        {
            var apprenticeship = await OnGetAsync(Client);

            if (apprenticeship.RolesAndResponsibilitiesConfirmations.IsConfirmed())
            {
                return new RedirectToPageResult("Index", new { ApprenticeshipId });
            }

            RevisionId = apprenticeship.RevisionId;

            if (apprenticeship.RolesAndResponsibilitiesConfirmations.HasFlag(rolesAndResponsibilitiesConfirmation))
                SectionConfirmed = true;

            return Page();
        }

        protected async Task<IActionResult> SaveConfirmationStatus(RolesAndResponsibilitiesConfirmations rolesAndResponsibilitiesConfirmation)
        {
            if (!SectionConfirmed)
            {
                ModelState.AddModelError(nameof(SectionConfirmed), ConfirmationErrorMessage);
                return new PageResult();
            }

            await Client.ConfirmApprenticeship(ApprenticeshipId.Id, RevisionId,
                ApprenticeshipConfirmationRequest.ConfirmRolesAndResponsibilities(rolesAndResponsibilitiesConfirmation));

            return new RedirectToPageResult(NextPage, new { ApprenticeshipId });
        }
    }
}