using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
            // Post, Redirect, Get
            FirstName = firstName;
            LastName = lastName;
            NationalInsuranceNumber = nationalInsuranceNumber;

            Guid.TryParse(User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value ?? "", out var regId);
            var reg = await _registrations.GetRegistration(regId);

            if (reg.UserId != null) return RedirectToPage("overview");

            EmailAddress = reg?.EmailAddress;

            return Page();
        }

        public IActionResult OnPost()
        {
            return RedirectToPage("/ConfirmYourIdentity", new
            {
                FirstName,
                LastName,
                NationalInsuranceNumber,
            });
        }
    }
}