using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.UnitTests.Hooks
{
    public class ActionResultFilter : IAsyncAlwaysRunResultFilter
    {
        private readonly IHook<IActionResult> _actionResultHook;

        public ActionResultFilter(IHook<IActionResult> actionResultHook)
        {
            _actionResultHook = actionResultHook;
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            try
            {
                _actionResultHook?.OnReceived?.Invoke(context.Result);
                await next();
                _actionResultHook?.OnProcessed?.Invoke(context.Result);
            }
            catch (Exception ex)
            {
                _actionResultHook?.OnErrored?.Invoke(ex, context.Result);
                throw;
            }
        }
    }
}