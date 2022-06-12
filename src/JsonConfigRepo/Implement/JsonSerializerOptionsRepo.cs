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
        /// <summary>
        /// 默认的Json序列化配置库
        /// </summary>
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

        /// <inheritdoc />
        public JsonSerializerOptions UTF8Options { get; }

        /// <inheritdoc />
        public JsonSerializerOptions UTF8PrettyOptions { get; }

        /// <inheritdoc />
        public JsonSerializerOptions StandardOptions { get; }


        /// <inheritdoc />
        public JsonSerializerOptions DebugJsonOptions { get; }

        /// <inheritdoc />
        public JsonSerializerOptions DebugJsonPrettyOptions { get; }
    }
}