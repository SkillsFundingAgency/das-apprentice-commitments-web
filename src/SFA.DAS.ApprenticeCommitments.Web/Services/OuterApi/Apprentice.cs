﻿using System;

namespace SFA.DAS.ApprenticeCommitments.Web.Services.OuterApi
{
    public class Apprentice
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
    }
}