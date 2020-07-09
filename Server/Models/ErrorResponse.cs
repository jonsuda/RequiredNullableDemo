using RequiredNullableDemo.Infrastructure;

namespace RequiredNullableDemo.Models
{
    public class ErrorResponse
    {
        public static ErrorResponse FromException(HttpStatusCodeException exception) =>
            new ErrorResponse((int)exception.Code, exception.Message);

        public ErrorResponse(int statusCode, string message) =>
            (this.StatusCode, this.Message) = (statusCode, message);

        public int StatusCode { get; }

        public string Message { get; }
    }
}
