using System.Net.Http.Headers;
using SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships;
using TechTalk.SpecFlow;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace SFA.DAS.ApprenticeCommitments.Web.AcceptanceTests.Features
{
    [Binding]
    [Scope(Feature = "CannotConfirm")]
    public class CannotConfirmSteps : StepsBase
    {
        private readonly TestContext _context;
        private readonly RegisteredUserContext _userContext;
        private CannotConfirmApprenticeshipModel _cannotConfirmYourApprenticeshipModel;
        private long _apprenticeshipId;
        private string _hashedApprenticeshipId;
        private string _backlink;
        private string _returnToMyApprenticeship;

        public CannotConfirmSteps(TestContext context, RegisteredUserContext userContext) : base(context)
        {
            _context = context;
            _userContext = userContext;
            _apprenticeshipId = 1235;
            _hashedApprenticeshipId = _context.Hashing.HashValue(_apprenticeshipId);
            _backlink = $"/apprenticeships/{_hashedApprenticeshipId}";

            
        }

        [Given("the apprentice has logged in")]
        public void GivenTheApprenticeHasLoggedIn()
        {
            
        }
    }
}
