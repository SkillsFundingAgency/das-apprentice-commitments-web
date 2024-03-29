﻿using SFA.DAS.ApprenticeCommitments.Web.Identity;

namespace SFA.DAS.ApprenticeCommitments.Web.UnitTests.Features
{
    internal static class Urls
    {
        public static string ConfirmMyApprenticshipPage(HashedId forApprenticeship)
            => $"/apprenticeships/{forApprenticeship.Hashed}";
        public static string MyApprenticshipPage(HashedId forApprenticeship)
            => $"/apprenticeships/{forApprenticeship.Hashed}/view";
    }
}