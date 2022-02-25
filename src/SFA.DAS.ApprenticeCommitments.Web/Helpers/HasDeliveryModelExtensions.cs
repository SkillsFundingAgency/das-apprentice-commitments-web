using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;
using SFA.DAS.ApprenticeCommitments.Web.Services;

namespace SFA.DAS.ApprenticeCommitments.Web.Helpers
{
    public static class HasDeliveryModelExtensions
    {
        public static bool IsNormalDelivery(this IHaveDeliveryModel deliveryModel)
            => deliveryModel.DeliveryModel == DeliveryModel.Regular;

        public static bool IsAbnormalDelivery(this IHaveDeliveryModel deliveryModel)
            => !IsNormalDelivery(deliveryModel);
    }
}
