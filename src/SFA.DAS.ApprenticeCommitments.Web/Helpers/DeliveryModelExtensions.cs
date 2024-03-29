﻿using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;

namespace SFA.DAS.ApprenticeCommitments.Web.Helpers
{
    public static class DeliveryModelExtensions
    {
        public static string? AbnormalDisplayName(this DeliveryModel deliveryModel)
            => deliveryModel switch
            {
                DeliveryModel.Regular => null,
                _ => deliveryModel.DisplayName()
            };
    }
}