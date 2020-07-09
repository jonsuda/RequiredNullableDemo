using System;
using System.IO;
using System.Net;
using System.Text.Json;

namespace RequiredNullableDemo.Infrastructure
{
    public class Serializer
    {
        private static readonly Lazy<Serializer> instance =
            new Lazy<Serializer>(() => new Serializer());

        public static Serializer Instance => instance.Value;

        private readonly JsonSerializerOptions options;

        private Serializer()
        {
            this.options =
                new JsonSerializerOptions()
                {
                    IgnoreNullValues = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };
        }

        public string Serialize<TValue>(TValue value) =>
            JsonSerializer.Serialize(value, this.options);

        public TValue Deserialize<TValue>(Stream inputStream)
        {
            try
            {
                using var reader = new StreamReader(inputStream);
                return JsonSerializer.Deserialize<TValue>(
                    reader.ReadToEnd(), this.options);
            }
            catch (Exception exception)
            {
                throw new HttpStatusCodeException(
                    HttpStatusCode.BadRequest, exception.Message, exception);
            }
        }
    }
}
