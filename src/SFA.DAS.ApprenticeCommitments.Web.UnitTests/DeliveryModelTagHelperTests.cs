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

        [TestCase(DeliveryModel.Normal, TagContent)]
        [TestCase(DeliveryModel.Flexible, "")]
        public async Task Normal(DeliveryModel deliveryModel, string expected)
        {
            TagHelperOutput.Content.Append("some text");
            var sut = new NormalDeliveryModelTagHelper
            {
                AspNormalDelivery = new DeliveryModelContainer(deliveryModel),
            };

            await sut.ProcessAsync(TagHelperContext, TagHelperOutput);

            TagHelperOutput.Content.GetContent().Should().Be(expected);
        }

        [TestCase(DeliveryModel.Flexible, TagContent)]
        [TestCase(DeliveryModel.Normal, "")]
        public async Task Abnormal(DeliveryModel deliveryModel, string expected)
        {
            TagHelperOutput.Content.Append("some text");
            var sut = new AbnormalDeliveryModelTagHelper
            {
                AspAbnormalDelivery = new DeliveryModelContainer(deliveryModel),
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