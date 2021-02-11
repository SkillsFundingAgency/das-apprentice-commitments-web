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

        public int DayOfBirth { get; set; }
        public int MonthOfBirth { get; set; }
        public int YearOfBirth { get; set; }

        [BindProperty]
        [Required(
            AllowEmptyStrings = false,
            ErrorMessage = "Please enter your national insurance number")]
        public string NationalInsuranceNumber{ get; set; }

        public async Task<IActionResult> OnGetAsync(
            string firstName = null,
            string lastName = null,
            string nationalInsuranceNumber = null)
        {
            FirstName = firstName;
            LastName = lastName;
            NationalInsuranceNumber = nationalInsuranceNumber;

            // TODO - rework based on outcome of CS-213
            Guid.TryParse(User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value ?? "", out var regId);
            var reg = await _registrations.GetRegistration(regId);

            if (reg.UserId != null) return RedirectToPage("overview");

            EmailAddress = reg?.EmailAddress;

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            Guid.TryParse(User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value ?? "", out var regId);
            await _registrations.Validate(new VerifyRegistrationCommand
            {
                RegistrationId = regId,
                FirstName = FirstName,
                LastName = LastName,
                NationalInsuranceNumber = NationalInsuranceNumber,
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