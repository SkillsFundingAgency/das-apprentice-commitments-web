using System;
using System.Runtime.Serialization;

namespace SFA.DAS.ApprenticeCommitments.Web.Exceptions
{
    [Serializable]
    public class PropertyNullException : InvalidOperationException
    {
        public PropertyNullException(string propertyName) => PropertyName = propertyName;

        public string PropertyName { get; }

        public override string Message => $"Property '{PropertyName}'must not be null";

        protected PropertyNullException(SerializationInfo info, StreamingContext context)
        {
            _ = info ?? throw new ArgumentNullException(nameof(info));
            PropertyName = info.GetValue(nameof(PropertyName), typeof(string)) as string
                ?? throw new InvalidOperationException();
        }
    }
}