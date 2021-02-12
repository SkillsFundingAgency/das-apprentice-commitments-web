using Microsoft.AspNetCore.Mvc;
using SFA.DAS.ApprenticeCommitments.Web.AcceptanceTests.Hooks;
using System.Linq;

namespace SFA.DAS.ApprenticeCommitments.Web.AcceptanceTests.Steps
{
    public class StepsBase
    {
        public StepsBase(TestContext testContext)
        {
            var hook = testContext.Web.ActionResultHook;
            if (hook != null)
            {
                testContext.ActionResult = new TestActionResult();
                hook.OnProcessed = (actionResult) => { testContext.ActionResult.SetActionResult(actionResult); };
                hook.OnErrored = (ex, actionResult) => { testContext.ActionResult.SetException(ex); };
            }
        }
    }
}