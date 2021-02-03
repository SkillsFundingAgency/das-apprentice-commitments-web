using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace SFA.DAS.ApprenticeCommitments.Web.AcceptanceTests.Steps
{
    [Binding]
    [Scope(Feature = "NotAuthenticated")]
    public class NotAuthenticatedSteps : StepsBase
    {
        private readonly TestContext _context;

        public NotAuthenticatedSteps(TestContext context) : base(context)
        {
            _context = context;
        }


    }
}
