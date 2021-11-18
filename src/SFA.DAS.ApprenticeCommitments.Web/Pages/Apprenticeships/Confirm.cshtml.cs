using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SFA.DAS.ApprenticeCommitments.Web.Exceptions;
using SFA.DAS.ApprenticeCommitments.Web.Identity;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;
using SFA.DAS.ApprenticePortal.SharedUi.Identity;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships
{
    public enum ConfirmStatus
    {
        SectionsIncomplete,
        SectionsComplete,
        ApprenticeshipComplete,
    }

    [RequiresIdentityConfirmed]
    public class ConfirmApprenticeshipModel : PageModel
    {
        private readonly IOuterApiClient _client;
        private readonly AuthenticatedUser _authenticatedUser;
        private readonly ITimeProvider _time;
        private readonly ILogger<ConfirmApprenticeshipModel> _logger;

        [BindProperty(SupportsGet = true)]
        public HashedId ApprenticeshipId { get; set; }

        [BindProperty]
        public long RevisionId { get; set; }

        public int DaysRemaining { get; set; }
        public bool Overdue => DaysRemaining <= 0;

        public bool? EmployerConfirmation { get; set; } = null;
        public bool? TrainingProviderConfirmation { get; set; } = null;
        public bool? ApprenticeshipDetailsConfirmation { get; set; } = null;
        public bool? RolesAndResponsibilitiesConfirmation { get; set; } = null;
        public bool? HowApprenticeshipWillBeDeliveredConfirmation { get; set; } = null;

        public ChangeOfCircumstanceNotifications ChangeNotifications { get; set; }
        public bool ShowChangeNotification => ChangeNotifications != ChangeOfCircumstanceNotifications.None;

        public string ChangeNotificationsMessage => BuildChangeNotificationMessage();

        private string BuildChangeNotificationMessage()
        {

            if (ChangeNotifications == ChangeOfCircumstanceNotifications.ApprenticeshipDetailsChanged)
            {
                return "The details of your apprenticeship have been corrected. Please review and confirm the changes to your apprenticeship details.";
            }
            
            var message = "Your ";
            switch (ChangeNotifications)
            {
                case ChangeOfCircumstanceNotifications.ProviderDetailsChanged | ChangeOfCircumstanceNotifications.EmployerDetailsChanged | ChangeOfCircumstanceNotifications.ApprenticeshipDetailsChanged:
                    message += "training provider, employer and apprenticeship";
                    break;
                case ChangeOfCircumstanceNotifications.ProviderDetailsChanged | ChangeOfCircumstanceNotifications.EmployerDetailsChanged:
                    message += "training provider and employer";
                    break;
                case ChangeOfCircumstanceNotifications.ProviderDetailsChanged | ChangeOfCircumstanceNotifications.ApprenticeshipDetailsChanged:
                    message += "training provider and apprenticeship";
                    break;
                case ChangeOfCircumstanceNotifications.EmployerDetailsChanged | ChangeOfCircumstanceNotifications.ApprenticeshipDetailsChanged:
                    message += "employer and apprenticeship";
                    break;
                case ChangeOfCircumstanceNotifications.ProviderDetailsChanged:
                    message += "training provider";
                    break;
                case ChangeOfCircumstanceNotifications.EmployerDetailsChanged:
                    message += "employer";
                    break;
                default:
                    throw new ApplicationException($"ChangeNotification Type {ChangeNotifications} not found");
            }

            return message + " details have been corrected. Please review and confirm the changes to your apprenticeship details.";
        }

        public bool ApprenticeshipConfirmed => Status == ConfirmStatus.ApprenticeshipComplete;

        public bool AllConfirmed => Status == ConfirmStatus.SectionsComplete;

        public ConfirmStatus Status { get; private set; }

        public string Forwardlink => $"/apprenticeships/{ApprenticeshipId.Hashed}/transactioncomplete";

        public ConfirmApprenticeshipModel(IOuterApiClient client, ITimeProvider time, AuthenticatedUser authenticatedUser, ILogger<ConfirmApprenticeshipModel> logger)
        {
            _client = client;
            _time = time;
            _authenticatedUser = authenticatedUser;
            _logger = logger;
        }

        public async Task OnGetAsync()
        {
            if (ApprenticeshipId == default)
                throw new PropertyNullException(nameof(ApprenticeshipId));

            var apprenticeship = await _client
                .GetApprenticeship(_authenticatedUser.ApprenticeId, ApprenticeshipId.Id);

            Status = ConfirmationStatus(apprenticeship);
            DaysRemaining = CalculateDaysRemaining(apprenticeship);

            RevisionId = apprenticeship.RevisionId;
            EmployerConfirmation = apprenticeship.EmployerCorrect;
            TrainingProviderConfirmation = apprenticeship.TrainingProviderCorrect;
            ApprenticeshipDetailsConfirmation = apprenticeship.ApprenticeshipDetailsCorrect;
            RolesAndResponsibilitiesConfirmation = apprenticeship.RolesAndResponsibilitiesConfirmations.IsConfirmed() ? true : (bool?)null;
            HowApprenticeshipWillBeDeliveredConfirmation = apprenticeship.HowApprenticeshipDeliveredCorrect;
            ChangeNotifications = apprenticeship.ChangeOfCircumstanceNotifications;

            ViewData[ApprenticePortal.SharedUi.ViewDataKeys.MenuWelcomeText] = $"Welcome, {User.FullName()}";

            _logger.LogInformation($"Marking apprenticeship as viewed {_authenticatedUser.ApprenticeId}, {ApprenticeshipId.Id}");
            await _client.UpdateRevisionLastViewed(_authenticatedUser.ApprenticeId, ApprenticeshipId.Id, RevisionId);
        }

        private ConfirmStatus ConfirmationStatus(Apprenticeship apprenticeship)
        {
            if (apprenticeship.ConfirmedOn.HasValue)
            {
                return ConfirmStatus.ApprenticeshipComplete;
            }
            else if (
                apprenticeship.EmployerCorrect == true &&
                apprenticeship.TrainingProviderCorrect == true &&
                apprenticeship.ApprenticeshipDetailsCorrect == true &&
                apprenticeship.RolesAndResponsibilitiesConfirmations.IsConfirmed() &&
                apprenticeship.HowApprenticeshipDeliveredCorrect == true)
            {
                return ConfirmStatus.SectionsComplete;
            }
            else
            {
                return ConfirmStatus.SectionsIncomplete;
            }
        }

        private int CalculateDaysRemaining(Apprenticeship apprenticeship)
        {
            // Show "1 day remaining during" the last hours of the last day, when technically
            // there is less that one whole day.
            var daysRemaining = apprenticeship.ConfirmBefore.AddDays(1) - _time.Now;
            return Math.Max(0, daysRemaining.Days);
        }

        public async Task<IActionResult> OnPostConfirm()
        {
            await _client.ConfirmApprenticeship(
                _authenticatedUser.ApprenticeId, ApprenticeshipId.Id, RevisionId,
                new ApprenticeshipConfirmationRequest(true));

            return Redirect(Forwardlink);
        }

        public string Pluralise(int number, string singular) =>
            $"{number} {singular}{(number == 1 ? "" : "s")}";
    }
}