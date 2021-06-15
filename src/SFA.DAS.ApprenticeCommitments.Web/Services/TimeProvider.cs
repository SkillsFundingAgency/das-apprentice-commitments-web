using System;

namespace SFA.DAS.ApprenticeCommitments.Web.Services
{
    public interface ITimeProvider
    {
        DateTimeOffset Now { get; }
    }

    public class UtcTimeProvider : ITimeProvider
    {
        public DateTimeOffset Now => DateTimeOffset.UtcNow;
    }

    public class SpecifiedTimeProvider : ITimeProvider
    {
        public SpecifiedTimeProvider(DateTimeOffset time) => Now = time;

        public DateTimeOffset Now { get; set; }

        public override string ToString() => Now.ToString();
    }
}