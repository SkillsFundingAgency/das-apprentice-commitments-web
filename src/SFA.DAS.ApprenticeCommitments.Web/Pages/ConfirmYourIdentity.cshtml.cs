using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SFA.DAS.ApprenticeCommitments.Web.Api;
using SFA.DAS.ApprenticeCommitments.Web.Api.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages
{
    [Authorize]
    public class ConfirmYourIdentityModel : PageModel
    {
        private readonly RegistrationsService _registrations;

        public ConfirmYourIdentityModel(RegistrationsService api) => _registrations = api;

        public string EmailAddress { get; set; }

        [BindProperty]
        [Required(
            AllowEmptyStrings = false,
            ErrorMessage = "Please enter your first name")]
        public string FirstName { get; set; }

        [BindProperty]
        [Required(
            AllowEmptyStrings = false,
            ErrorMessage = "Please enter your last name")]
        public string LastName { get; set; }

        [BindProperty]
        public DateModel DateOfBirth { get; set; }

        [BindProperty]
        [Required(
            AllowEmptyStrings = false,
            ErrorMessage = "Please enter your national insurance number")]
        public string NationalInsuranceNumber { get; set; }

        public async Task<IActionResult> OnGetAsync(
            [FromServices] RegistrationUser user,
            string firstName = null,
            string lastName = null,
            string nationalInsuranceNumber = null)
        {
            FirstName = firstName;
            LastName = lastName;
            NationalInsuranceNumber = nationalInsuranceNumber;

            var reg = await _registrations.GetRegistration(user);

            if (reg.UserId != null) return RedirectToPage("overview");

            EmailAddress = reg?.Email;

            return Page();
        }

        public async Task<IActionResult> OnPost([FromServices] RegistrationUser user)
        {
            await _registrations.Validate(new VerifyRegistrationCommand
            {
                RegistrationId = user.RegistrationId,
                FirstName = FirstName,
                LastName = LastName,
                NationalInsuranceNumber = NationalInsuranceNumber,
                DateOfBirth = DateOfBirth.Date,
            });

            return RedirectToPage("/ConfirmYourIdentity", new
            {
                FirstName,
                LastName,
                NationalInsuranceNumber,
            });
        }
    }
}