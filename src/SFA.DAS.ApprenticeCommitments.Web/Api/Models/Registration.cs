using System;

namespace SFA.DAS.ApprenticeCommitments.Web.Api.Models
{
    public class Registration
    {
        public Guid Id { get; set; }
        public int? UserId { get; set; }
        public string Email { get; set; }
    }
}