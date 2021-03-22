using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Threading.Tasks;
using SFA.DAS.ApprenticeCommitments.Web.Exceptions;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;

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

        [BindProperty]
        public string NationalInsuranceNumber { get; set; }

        public async Task<IActionResult> OnGetAsync(
            [FromServices] AuthenticatedUser user)
        {
            var reg = await _registrations.GetRegistration(user);

            if (reg.UserId != null) return RedirectToPage("/apprenticeships/index");

            EmailAddress = reg?.Email;

            return Page();
        }

        public async Task<IActionResult> OnPost([FromServices] AuthenticatedUser user)
        {
            try
            {
                await _registrations.VerifyRegistration(new VerifyRegistrationRequest
                {
                    RegistrationId = user.RegistrationId,
                    FirstName = FirstName,
                    LastName = LastName,
                    NationalInsuranceNumber = NationalInsuranceNumber,
                    DateOfBirth = DateOfBirth.IsValid ? DateOfBirth.Date : default,
                    UserIdentityId = Guid.NewGuid(),
                    Email = EmailAddress,
                });

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
                    nameof(NationalInsuranceNumber) => (e.PropertyName, "Enter your National Insurance Number"),
                    _ => ("", "Something went wrong"),
                };
                ModelState.AddModelError(p, m);
            }
        }
    }
}