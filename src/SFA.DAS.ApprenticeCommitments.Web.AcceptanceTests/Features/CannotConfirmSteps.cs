using System;
using System.Collections.Generic;
using SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships;
using System.Text;
using TechTalk.SpecFlow;

namespace SFA.DAS.ApprenticeCommitments.Web.AcceptanceTests.Features
{
    [Binding]
    [Scope(Feature = "CannotConfirm")]
    public class CannotConfirmSteps
    {
        private readonly TestContext _context;
        private readonly RegisteredUserContext _userContext;
        private CannotConfirmApprenticeshipModel _cannotConfirmYourApprenticeshipModel;
        private long _apprenticeshipId;
        private string _hashedApprenticeshipId;
        private string _backlink;
        private string _returnToMyApprenticeship;

    }
}
