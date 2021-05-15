using System;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages
{
    /// <summary>
    ///     Encapsulates a date whilst exposing separate day, month and year properties that can
    ///     be set independently.  The three elements are validated when set but may not be valid
    ///     when used together, for example 31-Feb. In this case accessing the <see cref="Date"/>
    ///     will throw an exception. To avoid this check the <see cref="IsValid"/> property before
    ///     accessing <see cref="Date"/>.
    /// </summary>
    public class DateModel
    {
        public DateModel()
        {
        }

        public DateModel(DateTime date) : this(date.Year, date.Month, date.Day)
        {
        }

        public DateModel(int year, int month, int day) =>
            (Year, Month, Day) = (year, month, day);

        public virtual int Day { get; set; }
        public virtual int Month { get; set; }
        public virtual int Year { get; set; }

        public DateTime Date =>
            IsValid ? new DateTime(Year, Month, Day) : default;

        public bool IsValid => Validate() == null;

        private Exception? Validate()
        {
            if (!IsValidDay)
                return new ArgumentException($"`{Day:00}` is not a valid day");
            if (!IsValidMonth)
                return new ArgumentException($"`{Month:00}` is not a valid month");
            if (!IsValidYear)
                return new ArgumentException($"`{Year:0000}` is not a valid year");
            if (Day > DateTime.DaysInMonth(Year, Month))
                return new ArgumentException($"`{Day}` is not a valid day in `{Year}-{Month:00}`");
            return null;
        }

        private bool IsValidDay =>
            Day > 0 &&
            Day <= 31;

        private bool IsValidMonth =>
            Month > 0 &&
            Month <= 12;

        private bool IsValidYear =>
            Year >= DateTime.MinValue.Year &&
            Year <= DateTime.MaxValue.Year;
    }
}