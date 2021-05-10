using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SFA.DAS.ApprenticeCommitments.Web.Exceptions;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages
{
    public class ConfirmYourIdentityModel : PageModel
    {
        private readonly RegistrationsService _registrations;

        public ConfirmYourIdentityModel(RegistrationsService api)
        {
            _registrations = api;
        }

        [BindProperty]
        [HiddenInput]
        public string EmailAddress { get; set; }

        [BindProperty]
        public string FirstName { get; set; }

        [BindProperty]
        public string LastName { get; set; }

        [BindProperty]
        public DateModel DateOfBirth { get; set; }

        public async Task<IActionResult> OnGetAsync(
            [FromServices] AuthenticatedUser user)
        {
            var reg = await _registrations.GetRegistration(user);

            if (reg.HasCompletedVerification)
            {
                return RedirectToPage("/apprenticeships/index");
            }

            if (!reg.HasViewedVerification)
            {
                await _registrations.FirstSeenOn(user.ApprenticeId, DateTime.UtcNow);
            }

            EmailAddress = reg?.Email;

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

            foreach (var e in exception.Errors)
            {
                var (p, m) = e.PropertyName switch
                {
                    nameof(FirstName) => (e.PropertyName, "Enter your first name"),
                    nameof(LastName) => (e.PropertyName, "Enter your last name"),
                    nameof(DateOfBirth) => (e.PropertyName, "Enter your date of birth"),
                    _ => ("", "Something went wrong"),
                };
                ModelState.AddModelError(p, m);
            }
        }
    }
}