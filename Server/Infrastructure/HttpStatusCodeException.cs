using System;
using System.Net;

namespace RequiredNullableDemo.Infrastructure
{
    public class HttpStatusCodeException : Exception
    {
        public HttpStatusCodeException(
            HttpStatusCode code, string message, Exception? innerException = null)
            : base(message, innerException) => this.Code = code;

        public HttpStatusCode Code { get; }
    }
}
