using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;

namespace SFA.DAS.Apprentice.SharedUi.GoogleAnalytics
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class EnableGoogleAnalyticsAttribute : ResultFilterAttribute
    {
        public GoogleAnalyticsConfiguration GoogleAnalyticsConfiguration { get; }

        public EnableGoogleAnalyticsAttribute(GoogleAnalyticsConfiguration configuration)
        {
            GoogleAnalyticsConfiguration = configuration;
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Controller is PageModel page)
                SetViewData(page.ViewData);

            if (context.Controller is Controller controller)
                SetViewData(controller.ViewData);

            void SetViewData(ViewDataDictionary viewData)
                => viewData[ViewDataKeys.GoogleAnalyticsConfiguration] = GoogleAnalyticsConfiguration;
        }
    }
}