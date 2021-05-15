using System;
using System.Collections.Generic;

namespace SFA.DAS.ApprenticeCommitments.Web.Exceptions
{
    public class DomainValidationException : Exception
    {
        public List<ErrorItem> Errors { get; }

        public DomainValidationException(List<ErrorItem> errors) : base("DomainValidation Exception")
        {
            Errors = errors;
        }
    }

    public class ErrorItem
    {
        public string PropertyName { get; set; } = null!;
        public string ErrorMessage { get; set; } = null!;
    }
}