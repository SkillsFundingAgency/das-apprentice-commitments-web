using NUnit.Framework.Constraints;

namespace SFA.DAS.Apprentice.SharedUi.Tests
{
    public class Contains : NUnit.Framework.Contains
    {
        public static ContainsZendeskLabelsInJavascriptSnippet ZendeskLabels(string labels)
            => new ContainsZendeskLabelsInJavascriptSnippet(labels);

        public static ContainsZendeskSuggestionInJavascriptSnippet ZendeskSuggestion(string labels)
            => new ContainsZendeskSuggestionInJavascriptSnippet(labels);
    }

    public class ContainsZendeskLabelsInJavascriptSnippet : EqualConstraint
    {
        public ContainsZendeskLabelsInJavascriptSnippet(string labels)
            : base(Matches(labels))
        { }

        private static string Matches(string labels)
        {
            const string StartLabelSnipet = "<script type=\"text/javascript\">zE('webWidget', 'helpCenter:setSuggestions', { labels: [";
            const string EndLabelSnipet = "] });</script>";

            return StartLabelSnipet + labels + EndLabelSnipet;
        }
    }

    public class ContainsZendeskSuggestionInJavascriptSnippet : EqualConstraint
    {
        public ContainsZendeskSuggestionInJavascriptSnippet(string labels)
            : base(Matches(labels))
        { }

        private static string Matches(string labels)
            => $"<script type=\"text/javascript\">zE('webWidget', 'helpCenter:setSuggestions', {{ search: '{labels}' }});</script>";
    }
}