using Microsoft.AspNetCore.Razor.TagHelpers;
using System;

namespace SFA.DAS.ApprenticeCommitments.Web.TagHelpers
{
    [HtmlTargetElement(Attributes = DisabledAttributeName)]
    public class DisabledTagHelper : TagHelper
    {
        private const string DisabledAttributeName = "asp-disabled";

        [HtmlAttributeName(DisabledAttributeName)]
        public bool Disabled { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));
            _ = output ?? throw new ArgumentNullException(nameof(output));

            if (Disabled) output.Attributes.SetAttribute("disabled", null);
        }
    }
}