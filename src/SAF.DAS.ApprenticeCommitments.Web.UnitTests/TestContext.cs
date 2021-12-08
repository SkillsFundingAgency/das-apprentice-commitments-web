using SFA.DAS.ApprenticeCommitments.Web.Services;
using SFA.DAS.ApprenticeCommitments.Web.UnitTests.Hooks;
using SFA.DAS.HashingService;
using System;
using System.Net;
using SFA.DAS.ApprenticePortal.SharedUi.Menu;

namespace SFA.DAS.ApprenticeCommitments.Web.UnitTests
{
    public class TestContext
    {
        public ApprenticeCommitmentsWeb Web { get; set; }
        public MockApi OuterApi { get; set; }
        public TestActionResult ActionResult { get; set; }
        public string IdentityServiceUrl { get; } = "https://identity";
        public IHashingService Hashing { get; set; }
        public SpecifiedTimeProvider Time { get; set; }
            = new SpecifiedTimeProvider(DateTimeOffset.UtcNow);

        public void ClearCookies()
        {
            var address = new Uri(OuterApi.BaseAddress);
            var cookies = Web.Cookies.GetCookies(address);
            foreach (Cookie cookie in cookies) cookie.Expired = true;
        }
    }

    public class RegisteredUserContext
    {
        public Guid ApprenticeId { get; set; } = Guid.NewGuid();
    }
}