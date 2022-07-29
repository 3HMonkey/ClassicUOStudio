using Microsoft.Xna.Framework;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ClassicUO.Utility.JSON
{
    public class Point2DConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert) =>
            typeToConvert == typeof(Point);

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options) =>
            new Point2DConverter();
    }
}