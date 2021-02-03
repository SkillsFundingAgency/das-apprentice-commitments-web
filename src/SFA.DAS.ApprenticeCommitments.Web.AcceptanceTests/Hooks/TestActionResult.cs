using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SFA.DAS.ApprenticeCommitments.Web.AcceptanceTests.Hooks
{
    public class TestActionResult
    {
        public IActionResult LastActionResult { get; private set; }
        public ViewResult LastViewResult { get; private set; }
        public PageResult LastPageResult { get; private set; }
        public Exception LastException { get; private set; }

        public TestActionResult()
        {
            LastActionResult = null;
            LastViewResult = null;
            LastException = null;
        }

        public void SetActionResult(IActionResult actionResult)
        {
            LastActionResult = actionResult;
            if (actionResult is ViewResult viewResult)
            {
                LastViewResult = viewResult;
            }
            else if (actionResult is PageResult pageResult)
            {
                LastPageResult = pageResult;
            }

        }

        public void SetException(Exception exception)
        {
            LastException = exception;
        }
    }
}
