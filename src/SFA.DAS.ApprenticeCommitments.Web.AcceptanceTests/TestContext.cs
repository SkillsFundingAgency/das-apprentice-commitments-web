using SFA.DAS.ApprenticeCommitments.Web.AcceptanceTests.Hooks;
using SFA.DAS.HashingService;
using System;

namespace SFA.DAS.ApprenticeCommitments.Web.AcceptanceTests
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
        public Guid RegistrationId { get; set; } = Guid.NewGuid();
    }
}