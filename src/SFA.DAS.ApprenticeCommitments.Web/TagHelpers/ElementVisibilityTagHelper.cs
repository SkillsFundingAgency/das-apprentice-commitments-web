using Microsoft.AspNetCore.Razor.TagHelpers;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;

namespace SFA.DAS.ApprenticeCommitments.Web.TagHelpers
{
    [HtmlTargetElement(Attributes = "asp-hide")]
    public class HideTagHelper : TagHelper
    {
        public bool? AspHide { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (AspHide ?? false) output.SuppressOutput();
        }
    }

    [HtmlTargetElement(Attributes = "asp-show")]
    public class ShowTagHelper : TagHelper
    {
        public bool? AspShow { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!(AspShow ?? false)) output.SuppressOutput();
        }
    }

    [HtmlTargetElement(Attributes = "asp-normal-delivery")]
    public class NormalDeliveryModelTagHelper : TagHelper
    {
        public IHaveDeliveryModel? AspNormalDelivery { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (AspNormalDelivery?.DeliveryModel != DeliveryModel.Normal)
                output.SuppressOutput();
        }

    }
    
    [HtmlTargetElement(Attributes = "asp-abnormal-delivery")]
    public class AbnormalDeliveryModelTagHelper : TagHelper
    {
        public IHaveDeliveryModel? AspAbnormalDelivery { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (AspAbnormalDelivery?.DeliveryModel == DeliveryModel.Normal)
                output.SuppressOutput();
        }

    }
}