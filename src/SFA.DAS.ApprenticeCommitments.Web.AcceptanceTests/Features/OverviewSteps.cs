using FluentAssertions;
using SFA.DAS.ApprenticeCommitments.Web.Pages;
using System;
using System.Net;
using TechTalk.SpecFlow;

namespace SFA.DAS.ApprenticeCommitments.Web.AcceptanceTests.Features
{
    [Binding]
    public class OverviewSteps : StepsBase
    {
        private readonly TestContext _context;
        private Guid _registrationId = Guid.NewGuid();
        private ConfirmYourIdentityModel _postedRegistration;

        public OverviewSteps(TestContext context) : base(context)
        {
            _context = context;
        }

        [Given(@"there is one apprenticeship")]
        public void GivenThereIsOneApprenticeship()
        {
        }

        [Then(@"the apprentice should see the overview page for their apprenticeship")]
        public void ThenTheApprenticeShouldSeeTheOverviewPage()
        {
            var page = _context.ActionResult.LastPageResult;
            page.Model.Should().BeOfType<OverviewModel>().Which.ApprenticeshipId.Should().Be(1234);
        }
    }
}