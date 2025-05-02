using System;

namespace SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi
{
    public class Timeline
    {
        public Timeline() { }

        public Timeline(string? heading, string? description, DateTime? revisionDate) =>
            (Heading, Description, RevisionDate) = (heading, description, revisionDate);

        public string? Heading { get; set; } = "";
        public string? Description { get; set; } = "";
        public DateTime? RevisionDate { get; set; } = null;
    }
}
