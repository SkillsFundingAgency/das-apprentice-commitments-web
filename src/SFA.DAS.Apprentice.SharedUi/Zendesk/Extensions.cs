using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace SFA.DAS.Apprentice.SharedUi.Zendesk
{
    public static class Extensions
    {
        public static string? GetZenDeskSectionId(this ViewDataDictionary viewData)
            => GetZendeskConfiguration(viewData)?.ZendeskSectionId;

        public static string? GetZenDeskSnippetKey(this ViewDataDictionary viewData)
            => GetZendeskConfiguration(viewData)?.ZendeskSnippetKey;

        public static string? GetCobrowsingSnippetKey(this ViewDataDictionary viewData)
            => GetZendeskConfiguration(viewData)?.ZendeskCobrowsingSnippetKey;

        private static ZenDeskConfiguration? GetZendeskConfiguration(ViewDataDictionary viewData)
            => viewData.TryGetValue(ViewDataKeys.ZenDeskConfiguration, out var section)
                ? section as ZenDeskConfiguration
                : null;

        public static HtmlString SetZendeskSuggestion(this IHtmlHelper html, string suggestion)
            => new HtmlString($"<script type=\"text/javascript\">zE('webWidget', 'helpCenter:setSuggestions', {{ search: '{EscapeApostrophes(suggestion)}' }});</script>");

        public static IHtmlContent SetZenDeskLabels(this IHtmlHelper html, params string[] labels)
        {
            var keywords = string.Join(",", labels
                .Where(label => !string.IsNullOrEmpty(label))
                .Select(label => $"'{EscapeApostrophes(label)}'"));

            // when there are no keywords default to empty string to prevent zen desk matching articles from the url
            var apiCallString = "<script type=\"text/javascript\">zE('webWidget', 'helpCenter:setSuggestions', { labels: ["
                    + (!string.IsNullOrEmpty(keywords) ? keywords : "''")
                               + "] });</script>";

            return new HtmlString(apiCallString);
        }

        private static string EscapeApostrophes(string input) => input.Replace("'", @"\'");

        public static IServiceCollection SetZenDeskConfiguration(this IServiceCollection services, ZenDeskConfiguration zenDeskConfiguration)
        {
            services.Configure<MvcOptions>(options =>
                options.Filters.Add(new SetZenDeskValuesAttribute(zenDeskConfiguration)));
            return services;
        }
    }
}