using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace SFA.DAS.ApprenticeCommitments.Web.TagHelpers
{
    public interface ISimpleUrlHelper
    {
        string Page(ActionContext context, string pageName, object values);
    }

    public class AspNetCoreSimpleUrlHelper : ISimpleUrlHelper
    {
        private readonly IUrlHelperFactory factory;

        public AspNetCoreSimpleUrlHelper(IUrlHelperFactory factory)
            => this.factory = factory;

        public string Page(ActionContext context, string pageName, object values)
            => factory.GetUrlHelper(context).Page(pageName, values);
    }
}