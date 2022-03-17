using FluentAssertions;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NUnit.Framework;
using SFA.DAS.ApprenticeCommitments.Web.Services;
using SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi;
using SFA.DAS.ApprenticeCommitments.Web.TagHelpers;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.UnitTests
{
    public class DeliveryModelTagHelperTests : TagHelperFixture
    {
        private const string TagContent = "some text";

        [TestCase(DeliveryModel.Regular, TagContent)]
        [TestCase(DeliveryModel.PortableFlexiJob, "")]
        public async Task Regular(DeliveryModel deliveryModel, string expected)
        {
            TagHelperOutput.Content.Append("some text");
            var sut = new NormalDeliveryModelTagHelper
            {
                AspRegularDelivery = new DeliveryModelContainer(deliveryModel),
            };

            await sut.ProcessAsync(TagHelperContext, TagHelperOutput);

            TagHelperOutput.Content.GetContent().Should().Be(expected);
        }

        [TestCase(DeliveryModel.PortableFlexiJob, TagContent)]
        [TestCase(DeliveryModel.Regular, "")]
        public async Task Irregular(DeliveryModel deliveryModel, string expected)
        {
            TagHelperOutput.Content.Append("some text");
            var sut = new AbnormalDeliveryModelTagHelper
            {
                AspIrregularDelivery = new DeliveryModelContainer(deliveryModel),
            };

            await sut.ProcessAsync(TagHelperContext, TagHelperOutput);

            TagHelperOutput.Content.GetContent().Should().Be(expected);
        }

        private class DeliveryModelContainer : IHaveDeliveryModel
        {
            public DeliveryModelContainer(DeliveryModel deliveryModel)
                => DeliveryModel = deliveryModel;

            public DeliveryModel DeliveryModel { get; set; }
        }
    }
}