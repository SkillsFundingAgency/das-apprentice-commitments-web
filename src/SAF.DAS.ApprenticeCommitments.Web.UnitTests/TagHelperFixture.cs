using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SAF.DAS.ApprenticeCommitments.Web.UnitTests
{
    public class TagHelperFixture
    {
        protected TagHelperContext TagHelperContext => new TagHelperContext(
                  new TagHelperAttributeList(),
                  new Dictionary<object, object>(),
                  Guid.NewGuid().ToString());

        protected TagHelperOutput TagHelperOutput = new TagHelperOutput("list",
                  new TagHelperAttributeList(),
                  (_, __) =>
                  {
                      var tagHelperContent = new DefaultTagHelperContent();
                      tagHelperContent.SetHtmlContent(string.Empty);
                      return Task.FromResult<TagHelperContent>(tagHelperContent);
                  });
    }
}