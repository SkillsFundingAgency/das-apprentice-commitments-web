using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SFA.DAS.Encoding;

namespace SFA.DAS.ApprenticeCommitments.Web.Identity
{
    internal class HashedIdRouteConstraint : IRouteConstraint
    {
        private readonly IEncodingService _hasher;

        public HashedIdRouteConstraint(IEncodingService hasher) => _hasher = hasher;

        public bool Match(HttpContext? httpContext, IRouter? route, string routeKey, 
            RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (!values.TryGetValue(routeKey, out var value)) return false;
            if (!(value is string possibleHash)) return false;
            return _hasher.TryDecode(possibleHash, EncodingType.ApprenticeshipId, out _);
        }
    }
}