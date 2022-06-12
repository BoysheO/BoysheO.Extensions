using System.Collections.Generic;
using System.Text.Json.Serialization;
using JsonConfigRepo.Abstractions;

namespace JsonConfigRepo.Implement
{
    /// <summary>
    /// 提供JsonConverter
    /// </summary>
    public sealed class JsonConvertRepo : IJsonConvertRepo
    {
        /// <summary>
        /// 提供用户自定义的JsonConverter
        /// </summary>
        public JsonConvertRepo(IReadOnlyCollection<JsonConverter> converters)
        {
            Converters = converters;
        }

        /// <inheritdoc/>
        public IReadOnlyCollection<JsonConverter> Converters { get; }
    }
}