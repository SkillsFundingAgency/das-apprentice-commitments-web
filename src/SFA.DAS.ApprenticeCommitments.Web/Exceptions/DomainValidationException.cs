using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SFA.DAS.ApprenticeCommitments.Web.Exceptions
{
    [Serializable]
    public class DomainValidationException : Exception
    {
        public List<ErrorItem> Errors { get; }

        public DomainValidationException(List<ErrorItem> errors) : base("DomainValidation Exception")
        {
            Errors = errors;
        }

        protected DomainValidationException(SerializationInfo info, StreamingContext context)
        : base(info, context)
        {
            _ = info ?? throw new ArgumentNullException(nameof(info));
            Errors = info.GetValue(nameof(Errors), typeof(List<ErrorItem>)) as List<ErrorItem>
                ?? throw new InvalidOperationException();
        }
    }

    public class ErrorItem
    {
        public string PropertyName { get; set; } = null!;
        public string ErrorMessage { get; set; } = null!;
    }
}