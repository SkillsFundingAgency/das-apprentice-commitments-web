using AngleSharp;
using AngleSharp.Dom;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.ApprenticeCommitments.Web.AcceptanceTests
{
    internal static class HtmlHelpers
    {
        public static async Task<IDocument> GetDocumentAsync(HttpResponseMessage response)
        {
            var contentStream = await response.Content.ReadAsStreamAsync();

            var browser = BrowsingContext.New();

            var document = await browser.OpenAsync(virtualResponse =>
            {
                virtualResponse.Content(contentStream, shouldDispose: true);
                virtualResponse.Address(response.RequestMessage.RequestUri).Status(response.StatusCode);
            });

            return document;
        }
    }
}