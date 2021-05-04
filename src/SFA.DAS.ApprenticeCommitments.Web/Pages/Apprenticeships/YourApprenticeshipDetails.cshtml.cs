using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SFA.DAS.ApprenticeCommitments.Web.Identity;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships
{
    [RequiresIdentityConfirmed]
    public class YourApprenticeshipDetails : PageModel, IHasBackLink
    {
        private readonly IOuterApiClient _client;
        private readonly AuthenticatedUser _authenticatedUser;

        [BindProperty(SupportsGet = true)]
        public HashedId ApprenticeshipId { get; set; }

        [BindProperty]
        public bool? ConfirmedApprenticeshipDetails { get; set; }
        public string Backlink => $"/apprenticeships/{ApprenticeshipId.Hashed}";

        [BindProperty]
        public string CourseName { get; set; }
        [BindProperty]
        public int CourseLevel { get; set; }
        [BindProperty]
        public string? CourseOption { get; set; }
        [BindProperty]
        public int DurationInMonths { get; set; }
        [BindProperty]
        public DateTime PlannedStartDate { get; set; }
        [BindProperty]
        public DateTime PlannedEndDate { get; set; }

        public YourApprenticeshipDetails(IOuterApiClient client, AuthenticatedUser authenticatedUser)
        {
            _client = client;
            _authenticatedUser = authenticatedUser;
        }

        public async Task OnGet()
        {
            var apprenticeship = await _client
                .GetApprenticeship(_authenticatedUser.ApprenticeId, ApprenticeshipId.Id);
            
            CourseName = apprenticeship.CourseName;
            CourseLevel = apprenticeship.CourseLevel;
            CourseOption = apprenticeship.CourseOption;
            DurationInMonths = apprenticeship.DurationInMonths;
            PlannedStartDate = apprenticeship.PlannedStartDate;
            PlannedEndDate = apprenticeship.PlannedEndDate;

            if(apprenticeship.ApprenticeshipDetailsCorrect == true)
                ConfirmedApprenticeshipDetails = true;
        }

        public async Task<IActionResult> OnPost()
        {
            if (ConfirmedApprenticeshipDetails == null)
            {
                ModelState.AddModelError(nameof(ConfirmedApprenticeshipDetails), "Select an answer");
                return new PageResult();
            }

            await _client.ConfirmApprenticeshipDetails(
                _authenticatedUser.ApprenticeId, ApprenticeshipId.Id,
                new ApprenticeshipDetailsConfirmationRequest(ConfirmedApprenticeshipDetails.Value));

            var nextPage = ConfirmedApprenticeshipDetails.Value ? "Confirm" : "CannotConfirm";

            return new RedirectToPageResult(nextPage, new { ApprenticeshipId });
        }
    }
}
