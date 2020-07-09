using System;

namespace RequiredNullableDemo.Infrastructure
{
    public class ValidationException : Exception
    {
        public ValidationException(params string[] validationMessages)
            : base("The validation of the request has failed.") =>
            this.ValidationMessages = validationMessages;

        public string[] ValidationMessages { get; }
    }
}
