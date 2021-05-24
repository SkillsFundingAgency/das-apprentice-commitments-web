using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SFA.DAS.HashingService;

namespace SFA.DAS.ApprenticeCommitments.Web.Identity
{
    internal class HashedIdRouteConstraint : IRouteConstraint
    {
        private readonly IHashingService _hasher;

        public HashedIdRouteConstraint(IHashingService hasher) => _hasher = hasher;

        public bool Match(HttpContext httpContext, IRouter route, string routeKey,
            RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (!values.TryGetValue(routeKey, out object value)) return false;
            if (!(value is string possibleHash)) return false;
            return _hasher.TryDecodeValue(possibleHash, out _);
        }
    }
}