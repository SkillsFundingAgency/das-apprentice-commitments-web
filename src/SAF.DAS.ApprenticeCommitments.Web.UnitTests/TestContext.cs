using SFA.DAS.ApprenticeCommitments.Web.UnitTests.Hooks;
using SFA.DAS.HashingService;
using System;

namespace SFA.DAS.ApprenticeCommitments.Web.UnitTests
{
    public class TestContext
    {
        public ApprenticeCommitmentsWeb Web { get; set; }
        public MockApi OuterApi { get; set; }
        public TestActionResult ActionResult { get; set; }
        public string IdentityServiceUrl { get; } = "https://identity";
        public IHashingService Hashing { get; set; }
    }

    public class RegisteredUserContext
    {
        public Guid ApprenticeId { get; set; } = Guid.NewGuid();
    }
}