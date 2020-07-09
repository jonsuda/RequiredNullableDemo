using RequiredNullableDemo.Infrastructure;

namespace RequiredNullableDemo.Models
{
    public class ValidationFailedResponse
    {
        public static ValidationFailedResponse
            FromException(ValidationException exception) =>
            new ValidationFailedResponse(exception);

        private ValidationFailedResponse(ValidationException exception)
        {
            this.Message = exception.Message;
            this.ValidationMessages = exception.ValidationMessages;
        }

        public string Message { get; }

        public string[] ValidationMessages { get; }
    }
}
