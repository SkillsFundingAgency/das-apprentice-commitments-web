using SFA.DAS.ApprenticeCommitments.Web.AcceptanceTests;
using SFA.DAS.ApprenticeCommitments.Web.AcceptanceTests.Features;
using SFA.DAS.ApprenticeCommitments.Web.Pages.IdentityHashing;
using System;
using System.Collections.Generic;
using System.Text;
using TechTalk.SpecFlow;

namespace SAF.DAS.ApprenticeCommitments.Web.UnitTests.Features
{
    [Binding]
    [Scope(Feature = "ConfirmYourEmployer")]
    public class HowYourApprenticeshipWillBeDeliveredSteps : StepsBase
    {
        private readonly TestContext _context;
        private readonly RegisteredUserContext _userContext;
        private HashedId _apprenticeshipId;
        private string _employerName;
        private bool? _employerNameConfirmed;
    }
}
