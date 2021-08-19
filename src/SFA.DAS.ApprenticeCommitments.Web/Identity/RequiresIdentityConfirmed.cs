using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SFA.DAS.ApprenticeCommitments.Web.Services;

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
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (UserAccountCreatedClaim.UserHasNotCreatedAccount(context.HttpContext))
                context.Result = new RedirectResult($"/Account{context.HttpContext.Request.QueryString}");
        }
    }
}