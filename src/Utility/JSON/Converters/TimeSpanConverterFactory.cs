using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ClassicUO.Utility.JSON
{
    public class TimeSpanConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert) => typeToConvert == typeof(TimeSpan);

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options) =>
            new TimeSpanConverter();
    }
}