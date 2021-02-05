using FluentAssertions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NUnit.Framework;
using SFA.DAS.ApprenticeCommitments.Web.Pages;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SAF.DAS.ApprenticeCommitments.Web.UnitTests
{
    public class WhenEnteringIdentityInformation
    {
        [Test, MoqAutoData]
        public void Shows_no_errors_when_all_fields_are_entered(ConfirmYourIdentityModel sut)
        {
            Validate(sut);
            sut.ModelState.IsValid.Should().BeTrue();
        }

        [Test, MoqAutoData]
        public void Shows_error_when_first_name_is_not_entered(ConfirmYourIdentityModel sut)
        {
            // Assemble
            sut.FirstName = null;

            // Action
            Validate(sut);

            // Assert
            sut.ModelState.IsValid.Should().BeFalse();

            sut.ModelState[nameof(sut.FirstName)].Errors.Should().ContainEquivalentOf(new
            {
                ErrorMessage = "Please enter your first name"
            });
        }

        [Test, MoqAutoData]
        public void Shows_error_when_last_name_is_not_entered(ConfirmYourIdentityModel sut)
        {
            // Assemble
            sut.LastName = null;

            // Action
            Validate(sut);

            // Assert
            sut.ModelState.IsValid.Should().BeFalse();

            sut.ModelState[nameof(sut.LastName)].Errors.Should().ContainEquivalentOf(new
            {
                ErrorMessage = "Please enter your last name"
            });
        }

        [Test, MoqAutoData]
        public void Shows_error_when_national_insurance_number_is_not_entered(ConfirmYourIdentityModel sut)
        {
            // Assemble
            sut.NationalInsuranceNumber = null;

            // Action
            Validate(sut);

            // Assert
            sut.ModelState.IsValid.Should().BeFalse();

            sut.ModelState[nameof(sut.NationalInsuranceNumber)].Errors.Should().ContainEquivalentOf(new
            {
                ErrorMessage = "Please enter your national insurance number"
            });
        }

        private static List<ValidationResult> Validate(PageModel sut)
        {
            var validator = new ValidationContext(sut);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(sut, validator, results, true);
            foreach(var r in results)
                sut.ModelState.AddModelError(r.MemberNames.First(), r.ErrorMessage);
            return results;
        }
    }
}