using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        private readonly Dictionary<Type, Func<JsonElement, object>> valueConverters;

        private Serializer()
        {
            this.options =
                new JsonSerializerOptions()
                {
                    IgnoreNullValues = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true
                };
            this.valueConverters = new Dictionary<Type, Func<JsonElement, object>>()
            {
                [typeof(Required<string>)] = this.ConvertString,
                [typeof(RequiredValue<int>)] = this.ConvertInt32,
                [typeof(RequiredValue<DateTime>)] = this.ConvertDateTime,
                [typeof(DateTime?)] = this.ConvertNullableDateTime
            };
        }

        public string Serialize<TValue>(TValue value) =>
            JsonSerializer.Serialize(value, this.options);

        public TValue Deserialize<TValue>(Stream inputStream)
            where TValue : new()
        {
            try
            {
                var value = new TValue();
                var propertiesByName =
                    typeof(TValue).GetProperties().ToDictionary(x => x.Name.ToLower());
                using var reader = new StreamReader(inputStream);
                var propertyValuesByName =
                    JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(
                        reader.ReadToEnd());
                foreach (var propertyValue in propertyValuesByName)
                {
                    if (propertiesByName.TryGetValue(
                        propertyValue.Key.ToLower(), out var property))
                    {
                        property.SetValue(
                            value,
                            this.valueConverters[property.PropertyType](propertyValue.Value));
                    }
                }
                return value;
            }
            catch (Exception exception)
            {
                throw new HttpStatusCodeException(
                    HttpStatusCode.BadRequest, exception.Message, exception);
            }
        }

        private object ConvertString(JsonElement element) =>
            new Required<string>(element.GetString());

        private object ConvertInt32(JsonElement element) =>
            new RequiredValue<int>(element.GetInt32());

        private object ConvertDateTime(JsonElement element) =>
            new RequiredValue<DateTime>(element.GetDateTime());

        private object ConvertNullableDateTime(JsonElement element) =>
            element.GetDateTime();
    }
}
