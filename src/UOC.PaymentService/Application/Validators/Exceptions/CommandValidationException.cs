using System;
using System.Runtime.Serialization;

namespace UOC.PaymentService.Validators
{
    [Serializable]
    public class CommandValidationException : Exception
    {
        public CommandValidationException(string message) : base(message) { }

        public CommandValidationException(string message, params object[] messageFormat) : base(string.Format(message, messageFormat)) { }

        protected CommandValidationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
