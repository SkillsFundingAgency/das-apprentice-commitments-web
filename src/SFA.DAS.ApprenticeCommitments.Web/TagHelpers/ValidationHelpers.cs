using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SFA.DAS.ApprenticeCommitments.Web.TagHelpers
{
    [HtmlTargetElement("div", Attributes = "validation-row-status")]
    public class ValidationRowHelper : TagHelper
    {
        [ViewContext]
        public ViewContext ViewContext { get; set; } = null!;

        public string PropertyName { get; set; } = null!;
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (PropertyIsInvalid())
                output.Attributes.Add("class", "govuk-form-group--error");
        }

        bool PropertyIsInvalid()
        {
            return ViewContext?.ModelState[PropertyName]?.ValidationState == ModelValidationState.Invalid;
        }
    }
}