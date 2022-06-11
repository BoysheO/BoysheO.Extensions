using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using JsonConfigRepo.Abstractions;

namespace JsonConfigRepo.Implement
{
    /// <summary>
    /// 默认的Json序列化配置库
    /// </summary>
    public sealed class JsonSerializerOptionsRepo : IJsonSerializerOptionsRepo
    {
        public JsonSerializerOptionsRepo(IJsonConvertRepo convertRepo)
        {
            var enumConvert = new JsonStringEnumConverter();
            StandardOptions = new JsonSerializerOptions();
            foreach (var converter in convertRepo.Converters) StandardOptions.Converters.Add(converter);

            UTF8Options = new JsonSerializerOptions(StandardOptions)
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            UTF8PrettyOptions = new JsonSerializerOptions(UTF8Options)
            {
                WriteIndented = true
            };

            DebugJsonOptions = new JsonSerializerOptions(StandardOptions)
            {
                IncludeFields = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            };
            if (!DebugJsonOptions.Converters.Contains(enumConvert))
            {
                DebugJsonOptions.Converters.Add(enumConvert);
            }

            DebugJsonPrettyOptions = new JsonSerializerOptions(DebugJsonOptions)
            {
                WriteIndented = true
            };
        }

        public JsonSerializerOptions UTF8Options { get; }
        public JsonSerializerOptions UTF8PrettyOptions { get; }
        public JsonSerializerOptions StandardOptions { get; }

        /// <summary>
        /// IncludeFields = true,
        /// Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        /// DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        /// DebugJsonOptions.Converters.Add(enumConvert);
        /// </summary>
        public JsonSerializerOptions DebugJsonOptions { get; }

        public JsonSerializerOptions DebugJsonPrettyOptions { get; }
    }
}