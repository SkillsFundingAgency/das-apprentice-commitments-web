using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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


    [HtmlTargetElement("yes-no-wrapper")]
    public class YesNoWrapHelper : TagHelper
    {
        [ViewContext]
        public ViewContext ViewContext { get; set; } = null!;
        public string PropertyName { get; set; } = null!;
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Attributes.Add("class", "govuk-form-group");
            output.PreContent.SetHtmlContent(@"<fieldset class=""govuk-fieldset"">");
            output.PostContent.SetHtmlContent("</fieldset>");
            
            if (PropertyIsInvalid()) {
                output.Attributes.RemoveAll("class");
                output.Attributes.Add("class", "govuk-form-group govuk-form-group--error");
            }
        }
        bool PropertyIsInvalid()
        {
            return ViewContext?.ModelState[PropertyName]?.ValidationState == ModelValidationState.Invalid;
        }
    }

    [HtmlTargetElement("yes-no-header")]
    public class YesNoHeaderHelper : TagHelper
    {
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "legend";
            output.Attributes.Add("class", "govuk-fieldset__legend govuk-fieldset__legend--m");
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
                        <input class=""govuk-radios__input"" id=""{For.Name}"" name=""{For.Name}"" type=""radio"" value=""true"" {YesAttribute}>
                        <label class=""govuk-label govuk-radios__label"" for=""{For.Name}"">
                            Yes
                        </label>
                    </div>
                    <div class=""govuk-radios__item"">
                        <input class=""govuk-radios__input"" id=""Not{For.Name}"" name=""{For.Name}"" type=""radio"" value=""false"">
                        <label class=""govuk-label govuk-radios__label"" for=""Not{For.Name}"">
                            No
                        </label>
                    </div>");
        }
    }
}