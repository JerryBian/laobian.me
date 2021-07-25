﻿using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Laobian.Share.Converter;

namespace Laobian.Share.Helper
{
    public static class JsonHelper
    {
        public static string Serialize<T>(T obj, bool writeIndented = false)
        {
            var option = new JsonSerializerOptions
            {
                WriteIndented = writeIndented,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            option.Converters.Add(new IsoDateTimeConverter());
            return JsonSerializer.Serialize(obj, option);
        }

        public static T Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json);
        }

        public static async Task<T> DeserializeAsync<T>(Stream stream)
        {
            return await JsonSerializer.DeserializeAsync<T>(stream);
        }
    }
}