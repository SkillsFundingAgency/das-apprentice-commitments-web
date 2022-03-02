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

    [HtmlTargetElement(Attributes = "asp-regular-delivery")]
    public class NormalDeliveryModelTagHelper : TagHelper
    {
        public IHaveDeliveryModel? AspNormalDelivery { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (AspNormalDelivery?.DeliveryModel != DeliveryModel.Regular)
                output.SuppressOutput();
        
            output.RemoveTagHelperTag();
        }

    }
    
    [HtmlTargetElement(Attributes = "asp-irregular-delivery")]
    public class AbnormalDeliveryModelTagHelper : TagHelper
    {
        public IHaveDeliveryModel? AspAbnormalDelivery { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (AspAbnormalDelivery?.DeliveryModel == DeliveryModel.Regular)
                output.SuppressOutput();

            output.RemoveTagHelperTag();
        }
    }

    public static class TagHelperExtensions
    {
        public static void RemoveTagHelperTag(this TagHelperOutput output)
        {
            // By convention, we use the tag name "x" when we only create a tag to
            // access the tag helper functionality
            if (output.TagName == "x")
                output.TagName = "";
        }
    }
}