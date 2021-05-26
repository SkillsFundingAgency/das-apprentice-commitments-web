using Microsoft.AspNetCore.Mvc.Rendering;
using NUnit.Framework;
using SFA.DAS.Apprentice.SharedUi.Zendesk;
using System;

namespace SFA.DAS.Apprentice.SharedUi.Tests
{
    [TestFixture]
    [Parallelizable]
    public class HttpHelperZendeskExtensionsTests
    {
        private readonly IHtmlHelper sut = null!;

        [Test]
        public void SetZendeskSuggestion_IsCreatedCorrectly()
        {
            const string suggestion = "iowoiwueoiwue";
            var htmlSnippet = sut.SetZendeskSuggestion(suggestion);

            Assert.That(htmlSnippet.ToString(), Contains.ZendeskSuggestion(suggestion));
        }

        [Test]
        public void SetZendeskSuggestion_IsCreatedCorrectlyWithApostrophesEscaped()
        {
            const string suggestion = "'help's";
            var htmlSnippet = sut.SetZendeskSuggestion(suggestion);

            Assert.That(htmlSnippet.ToString(), Contains.ZendeskSuggestion(@"\'help\'s"));
        }

        [Test]
        public void SetZendeskLabels_IsCreatedCorrectlyForNoItems()
        {
            var labels = Array.Empty<string>();
            var htmlSnippet = sut.SetZenDeskLabels(labels);

            Assert.That(htmlSnippet.ToString(), Contains.ZendeskLabels("''"));
        }

        [Test]
        public void SetZendeskLabels_IsCreatedCorrectlyForOneItem()
        {
            var labels = new[] { "one" };
            var htmlSnippet = sut.SetZenDeskLabels(labels);

            Assert.That(htmlSnippet.ToString(), Contains.ZendeskLabels("'one'"));
        }

        [Test]
        public void SetZendeskLabels_IsCreatedCorrectlyForTwoItems()
        {
            var labels = new[] { "one", "two" };
            var htmlSnippet = sut.SetZenDeskLabels(labels);

            Assert.That(htmlSnippet.ToString(), Contains.ZendeskLabels("'one','two'"));
        }

        [Test]
        public void SetZendeskLabels_IsCreatedCorrectlyForOneItemWithApostrophesEscaped()
        {
            var labels = new[] { "one's" };
            var htmlSnippet = sut.SetZenDeskLabels(labels);

            Assert.That(htmlSnippet.ToString(), Contains.ZendeskLabels(@"'one\'s'"));
        }
    }
}