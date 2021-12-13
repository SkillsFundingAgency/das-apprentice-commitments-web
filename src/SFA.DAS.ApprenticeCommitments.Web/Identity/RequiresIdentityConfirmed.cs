using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SFA.DAS.ApprenticePortal.Authentication;
using SFA.DAS.ApprenticePortal.SharedUi.Menu;

namespace SFA.DAS.ApprenticeCommitments.Web.Identity
{
    public class RequiresIdentityConfirmedAttribute : TypeFilterAttribute
    {
        public RequiresIdentityConfirmedAttribute() : base(typeof(ClaimRequirementFilter))
        {
        }
    }

    public class ClaimRequirementFilter : IAuthorizationFilter
    {
        private readonly NavigationUrlHelper _urlHelper;
        private readonly AuthenticatedUser _user;

        public ClaimRequirementFilter(NavigationUrlHelper urlHelper, AuthenticatedUser user)
        {
            _urlHelper = urlHelper;
            _user = user;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!_user.HasCreatedAccount)
                context.Result = new RedirectResult($"/Register{context.HttpContext.Request.QueryString}");

            else if (!_user.HasAcceptedTermsOfUse)
                context.Result = new RedirectResult(_urlHelper.Generate(NavigationSection.TermsOfUse));
        }
    }
}