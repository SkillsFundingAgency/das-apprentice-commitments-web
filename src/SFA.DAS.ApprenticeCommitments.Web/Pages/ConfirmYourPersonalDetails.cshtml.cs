using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SFA.DAS.ApprenticeCommitments.Web.Exceptions;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;
using SFA.DAS.ApprenticePortal.SharedUi.Menu;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages
{
    [HideNavigationBar]
    public class ConfirmYourPersonalDetailsModel : PageModel
    {
        private readonly RegistrationsService _registrations;

        public ConfirmYourPersonalDetailsModel(RegistrationsService api)
        {
            _registrations = api;
        }

        [BindProperty]
        [HiddenInput]
        public string EmailAddress { get; set; } = null!;

        [BindProperty]
        public string FirstName { get; set; } = null!;

        [BindProperty]
        public string LastName { get; set; } = null!;

        [BindProperty]
        public DateModel DateOfBirth { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(
            [FromServices] AuthenticatedUser user)
        {
            ViewData[ApprenticePortal.SharedUi.ViewDataKeys.MenuWelcomeText] = "Welcome";

            var registration = await _registrations.GetRegistration(user);

            if (registration.HasCompletedVerification)
            {
                return RedirectToPage("/apprenticeships/index");
            }

            if (!registration.HasViewedVerification)
            {
                await _registrations.FirstSeenOn(user.ApprenticeId, DateTime.UtcNow);
            }

            EmailAddress = registration.Email;

            return Page();
        }

        public async Task<IActionResult> OnPost([FromServices] AuthenticatedUser user)
        {
            try
            {
                await _registrations.VerifyRegistration(new VerifyRegistrationRequest
                {
                    ApprenticeId = user.ApprenticeId,
                    FirstName = FirstName,
                    LastName = LastName,
                    DateOfBirth = DateOfBirth.IsValid ? DateOfBirth.Date : default,
                    UserIdentityId = Guid.NewGuid(),
                    Email = EmailAddress,
                });

                await VerifiedUser.ConfirmIdentity(HttpContext);

                return RedirectToPage("/apprenticeships/index");
            }
            catch (DomainValidationException exception)
            {
                AddErrors(exception);
                return Page();
            }
        }

        private void AddErrors(DomainValidationException exception)
        {
            ModelState.ClearValidationState(nameof(DateOfBirth));
            ModelState.ClearValidationState(nameof(LastName));
            ModelState.ClearValidationState(nameof(FirstName));

            foreach (var e in exception.Errors)
            {
                var (p, m) = e.PropertyName switch
                {
                    nameof(FirstName) => (e.PropertyName, "Enter your first name"),
                    nameof(LastName) => (e.PropertyName, "Enter your last name"),
                    nameof(DateOfBirth) => (e.PropertyName, "Enter your date of birth"),
                    "PersonalDetails" => ("PersonalDetails", "Details entered do not match a registered apprenticeship. Please try again. If match continues to fail, contact your training provider to ensure they have given us the correct details."),
                    _ => ("", "Something went wrong")
                };
                if (p?.Length == 0 && ModelState.Keys.Contains("")) continue;
                ModelState.AddModelError(p, m);
            }
        }
    }
}