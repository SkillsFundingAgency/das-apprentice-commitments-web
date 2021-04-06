using SFA.DAS.ApprenticeCommitments.Web.AcceptanceTests;
using SFA.DAS.ApprenticeCommitments.Web.AcceptanceTests.Features;
using SFA.DAS.ApprenticeCommitments.Web.Pages.IdentityHashing;
using System.Net.Http.Headers;
using TechTalk.SpecFlow;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace SAF.DAS.ApprenticeCommitments.Web.UnitTests.Features
{
    [Binding]
    [Scope(Feature = "HowYourApprenticeshipWillBeDelivered")]
    public class HowYourApprenticeshipWillBeDeliveredSteps : StepsBase
    {
        private readonly TestContext _context;
        private readonly RegisteredUserContext _userContext;
        private HashedId _apprenticeshipId;

        public HowYourApprenticeshipWillBeDeliveredSteps(TestContext context, RegisteredUserContext userContext) : base(context)
        {
            _context = context;
            _userContext = userContext;
            _apprenticeshipId = HashedId.Create(1235, _context.Hashing);

            _context.OuterApi.MockServer.Given(
                     Request.Create()
                         .UsingAnyMethod()
                         .WithPath("/apprentices/*/apprenticeships/*/howapprenticeshipdeliveredconfirmation"))
                    .RespondWith(Response.Create()
                        .WithStatusCode(200));
        }

        [Given("the apprentice has logged in")]
        public void GivenTheApprenticeHasLoggedIn()
        {
            TestAuthenticationHandler.AddUser(_userContext.RegistrationId);
            _context.Web.Client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(_userContext.RegistrationId.ToString());
        }

        [Given(@"the apprentice has not verified they have read the page")]
        public void GivenTheApprenticeHasNotVerifiedTheyHaveReadThePage()
        {
        }
    }
}
