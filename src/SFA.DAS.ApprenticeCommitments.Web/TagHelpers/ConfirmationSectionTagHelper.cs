using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships;
using System;
using System.Threading.Tasks;

#nullable enable

namespace SFA.DAS.ApprenticeCommitments.Web.TagHelpers
{
    [HtmlTargetElement("li", Attributes = "confirmation-section")]
    public class ConfirmationSectionTagHelper : TagHelper
    {
        private readonly ISimpleUrlHelper _urlFactory;

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext? ViewContext { get; set; }

        public string? AspPage { get; set; }
        public ConfirmApprenticeshipModel? Model { get; set; }

        public ConfirmationSectionTagHelper(ISimpleUrlHelper urlFactory)
        {
            _urlFactory = urlFactory;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            _ = ViewContext ?? throw new ArgumentNullException(nameof(ViewContext));
            _ = AspPage ?? throw new ArgumentNullException(nameof(AspPage));
            _ = Model ?? throw new ArgumentNullException(nameof(Model));

            var url = _urlFactory.Page(ViewContext, AspPage, new { Model.ApprenticeshipId });

            bool? state = StateOf(AspPage);

            var colour = state switch
            {
                true => "red",
                false => "blue",
                null => "green",
            };

            var content = (await output.GetChildContentAsync()).GetContent();
            var encoded =
                $@"<a href=""{url}"" class=""app-status-list__link"">" +
                $@"<h3 class=""app-status-list__link-text;"" style=""background-color: {colour}"">{content}</h3>" +
                "</a>";
            output.Attributes.Add("class", "app-status-list__list-item");
            output.Content.SetHtmlContent(encoded);
        }

        private bool? StateOf(string aspPage)
        {
            var name = $"{aspPage.Replace("ConfirmYour", "")}Confirmation";
            var value = Model?.GetType().GetProperty(name);
            return value?.GetValue(Model) as bool?;
        }
    }
}