using SFA.DAS.ApprenticeCommitments.Web.Exceptions;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.ApprenticeCommitments.Web.Pages
{
    internal class ErrorItemComparePropertyName : IEqualityComparer<ErrorItem>
    {
        public bool Equals([AllowNull] ErrorItem x, [AllowNull] ErrorItem y)
            => x?.PropertyName.Equals(y?.PropertyName) ?? false;

        public int GetHashCode([DisallowNull] ErrorItem obj) => obj.PropertyName.GetHashCode();
    }
}