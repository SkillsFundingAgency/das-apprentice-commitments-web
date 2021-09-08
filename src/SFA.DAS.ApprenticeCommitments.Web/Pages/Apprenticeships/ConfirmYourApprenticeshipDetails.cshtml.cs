using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeCommitments.Web.Identity;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;
using SFA.DAS.ApprenticePortal.SharedUi.Menu;
using System;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships
{
    [RequiresIdentityConfirmed]
    [HideNavigationBar]
    public class YourApprenticeshipDetails : ApprenticeshipConfirmationPageModel
    {
        [BindProperty]
        public string CourseName { get; set; } = null!;

        [BindProperty]
        public int CourseLevel { get; set; }

        [BindProperty]
        public string? CourseOption { get; set; }

        [BindProperty]
        public int CourseDuration { get; set; }

        [BindProperty]
        public DateTime PlannedStartDate { get; set; }

        [BindProperty]
        public DateTime PlannedEndDate { get; set; }

        public YourApprenticeshipDetails(AuthenticatedUserClient client) : base("ApprenticeshipDetails", client)
        {
        }

        public override void LoadApprenticeship(Apprenticeship apprenticeship)
        {
            CourseName = apprenticeship.CourseName;
            CourseLevel = apprenticeship.CourseLevel;
            CourseOption = apprenticeship.CourseOption;
            CourseDuration = apprenticeship.CourseDuration;
            PlannedStartDate = apprenticeship.PlannedStartDate;
            PlannedEndDate = apprenticeship.PlannedEndDate;
            Confirmed = apprenticeship.ApprenticeshipDetailsCorrect;
        }

        public override ApprenticeshipConfirmationRequest CreateUpdate(bool confirmed)
            => ApprenticeshipConfirmationRequest.ConfirmApprenticeshipDetails(confirmed);
    }
}