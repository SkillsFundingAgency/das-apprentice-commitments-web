using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.TagHelpers
{
    [HtmlTargetElement("notification-banner")]
    public class ConfirmationBannerHelper : TagHelper
    {
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var content = (await output.GetChildContentAsync()).GetContent();

            output.TagName = "div";
            output.Attributes.Add("class", "app-notification-banner app-notification-banner--with-icon app-notification-banner--success");
            output.Content.SetHtmlContent($@"<span class=""app-notification-banner__icon das-text--success-icon""></span>{content}");
        }
    }

    [HtmlTargetElement("yes-no-header")]
    public class YesNoHeaderHelper : TagHelper
    {
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "fieldset";
            output.Attributes.Add("class", "govuk-fieldset");
            output.PreContent.SetHtmlContent(@"<legend class=""govuk-fieldset__legend govuk-fieldset__legend--l""><h2 class=""govuk-fieldset__heading"">");
            output.PostContent.SetHtmlContent("</h2></legend>");
        }
    }

    [HtmlTargetElement("yes-no-inputs")]
    public class YesNoInputsHelper : TagHelper
    {
        [HtmlAttributeName("asp-for")]
        public ModelExpression For { get; set; } = null!;

        public string YesAttribute => (For?.Model as bool?) == true ? "checked" : "";

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Attributes.Add("class", "govuk-radios govuk-radios--inline");
            output.Content.SetHtmlContent(
                $@"<div class=""govuk-radios__item"">
                        <input class=""govuk-radios__input"" id=""confirm-yes"" name=""{For.Name}"" type=""radio"" value=""true"" {YesAttribute}>
                        <label class=""govuk-label govuk-radios__label"" for=""confirm-yes"">
                            Yes
                        </label>
                    </div>
                    <div class=""govuk-radios__item"">
                        <input class=""govuk-radios__input"" id=""confirm-no"" name=""{For.Name}"" type=""radio"" value=""false"">
                        <label class=""govuk-label govuk-radios__label"" for=""confirm-no"">
                            No
                        </label>
                    </div>");
        }
    }
}