using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using SFA.DAS.ApprenticeCommitments.Web.Exceptions;
using SFA.DAS.ApprenticeCommitments.Web.Pages.Apprenticeships;
using System.Threading.Tasks;

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

        public bool? ConfirmationStatus { get; set; }

        public ConfirmationSectionTagHelper(ISimpleUrlHelper urlFactory)
        {
            _urlFactory = urlFactory;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            _ = ViewContext ?? throw new PropertyNullException(nameof(ViewContext));
            _ = AspPage ?? throw new PropertyNullException(nameof(AspPage));
            _ = Model ?? throw new PropertyNullException(nameof(Model));

            var url = _urlFactory.Page(ViewContext, AspPage, new { Model.ApprenticeshipId });

            var (stateClass, colourClass, tag) = ConfirmationStatus switch
            {
                true => ("complete", "green", "Complete"),
                false => ("incorrect", "red", "Waiting for<br/>correction"),
                null => ("", "yellow", "Incomplete"),
            };

            var content = (await output.GetChildContentAsync()).GetContent();
            var encoded =
                $@"<a href=""{url}"" class=""app-status-list__link {stateClass}"">" +
                $@"<div class=""app-status-list__link-content"">" +
                $@"<p class=""app-status-list__link-text"">{content}</p>" +
                $@"<strong class=""app-status-list__tag govuk-tag govuk-tag--{colourClass}"">{tag}</strong>" +
                "</div>" +
                "</a>";

            output.Attributes.RemoveAll("confirmation-section");
            output.Attributes.Add("class", "app-status-list__list-item");
            output.Content.SetHtmlContent(encoded);
        }
    }
}