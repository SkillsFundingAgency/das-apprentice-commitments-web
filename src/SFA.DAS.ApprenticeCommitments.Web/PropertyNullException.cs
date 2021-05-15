using System;

namespace SFA.DAS.ApprenticeCommitments.Web
{
    public class PropertyNullException : InvalidOperationException
    {
        public PropertyNullException(string propertyName) => PropertyName = propertyName;

        public string PropertyName { get; }

        public override string Message => $"Property '{PropertyName}'must not be null";
    }
}