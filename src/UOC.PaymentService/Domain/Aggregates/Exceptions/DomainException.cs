using System;
using System.Runtime.Serialization;

namespace UOC.PaymentService.Domain
{
    [Serializable]
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message) { }

        public DomainException(string message, params object[] messageFormat) : base(string.Format(message, messageFormat)) { }

        protected DomainException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
