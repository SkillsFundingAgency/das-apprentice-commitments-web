using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using NUnit.Framework;
using SFA.DAS.ApprenticeCommitments.Web.TagHelpers;

namespace SFA.DAS.ApprenticeCommitments.Web.UnitTests
{
    public class ConfirmTagHelperTests : TagHelperFixture
    {
        [Test, AutoData]
        public async Task Sets_name_of_form(string name)
        {
            var sut = new YesNoInputsHelper
            {
                For = new ModelExpression(
                    name: name,
                    modelExplorer: new EmptyModelMetadataProvider()
                                       .GetModelExplorerForType(typeof(bool?), false))
            };

            await sut.ProcessAsync(TagHelperContext, TagHelperOutput);

            var result = TagHelperOutput.Content.GetContent();
            result.Should().Contain($@"<input class=""govuk-radios__input"" id=""confirm-yes"" name=""{name}"" type=""radio"" ");
        }

        [TestCase(true, "checked")]
        [TestCase(false, "")]
        [TestCase(null, "")]
        public async Task Sets_checked_attribute_from_model_truthy(bool? model, string checkedAttribute)
        {
            var sut = new YesNoInputsHelper
            {
                For = new ModelExpression(
                    name: "MyModel",
                    modelExplorer: new EmptyModelMetadataProvider()
                                       .GetModelExplorerForType(typeof(bool?), model))
            };

            await sut.ProcessAsync(TagHelperContext, TagHelperOutput);

            sut.YesAttribute.Should().Be(checkedAttribute);
        }
    }
}