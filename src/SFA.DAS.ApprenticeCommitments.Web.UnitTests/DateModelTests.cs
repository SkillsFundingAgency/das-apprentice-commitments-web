using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.ApprenticeCommitments.Web.Pages;
using System;

namespace SFA.DAS.ApprenticeCommitments.Web.UnitTests
{
    public class DateModelTests
    {
        [Test]
        public void Create_with_default_constructor_is_invalid()
        {
            var sut = new DateModel();
            sut.IsValid.Should().BeFalse();
        }

        [Test, AutoData]
        public void Create_with_date_is_valid(DateTime date)
        {
            var sut = new DateModel(date);
            sut.IsValid.Should().BeTrue();
        }

        [Test, AutoData]
        public void Create_with_date_uses_date_components(DateTime date)
        {
            var sut = new DateModel(date);
            sut.Should().BeEquivalentTo(new
            {
                date.Day,
                date.Month,
                date.Year,
            });
        }

        [TestCase(2019, 1, 1)]
        [TestCase(2019, 12, 31)]
        public void Create_with_valid_dates_is_valid(int year, int month, int day)
        {
            var sut = new DateModel() { Day = day, Month = month, Year = year };
            sut.IsValid.Should().BeTrue();
        }

        [TestCase(2019, 01, -1)]
        [TestCase(2019, 02, 0)]
        [TestCase(2019, 02, 29)]
        [TestCase(2019, 06, 31)]
        [TestCase(2019, 03, 32)]
        [TestCase(2019, 00, 01)]
        [TestCase(2019, 13, 01)]
        [TestCase(0000, 01, 01)]
        public void Create_with_invalid_dates_is_not_valid(int year, int month, int day)
        {
            var sut = new DateModel() { Day = day, Month = month, Year = year };
            sut.IsValid.Should().BeFalse();
        }

        [Test, AutoData]
        public void Valid_models_Date_property_contains_date_components(DateTime date)
        {
            var sut = new DateModel(date);
            sut.Date.Should().Be(date.Date);
        }

        [TestCase(2020, 01, 99, "`99` is not a valid day")]
        [TestCase(2020, 99, 01, "`99` is not a valid month")]
        [TestCase(0000, 01, 01, "`0000` is not a valid year")]
        [TestCase(2020, 02, 30, "`30` is not a valid day in `2020-02`")]

        public void Invalid_models_Date_property_throws_exception(
            int year, int month, int day, string message)
        {
            var sut = new DateModel { Day = day, Month = month, Year = year };
            sut.Invoking(x => x.Date).Should().Throw<Exception>().WithMessage(message);
        }
    }
}
