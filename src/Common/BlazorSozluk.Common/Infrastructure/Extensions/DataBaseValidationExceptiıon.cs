using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BlazorSozluk.Common.Infrastructure.Extensions
{
    public class DataBaseValidationExceptiıon : Exception
    {
        public DataBaseValidationExceptiıon()
        {
        }

        public DataBaseValidationExceptiıon(string? message) : base(message)
        {
        }

        public DataBaseValidationExceptiıon(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected DataBaseValidationExceptiıon(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
