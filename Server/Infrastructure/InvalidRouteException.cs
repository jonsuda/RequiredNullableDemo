using System.Net;

namespace RequiredNullableDemo.Infrastructure
{
    public class InvalidRouteException : HttpStatusCodeException
    {
        public InvalidRouteException()
            : base(HttpStatusCode.BadRequest, "Invalid route.")
        { }
    }
}
