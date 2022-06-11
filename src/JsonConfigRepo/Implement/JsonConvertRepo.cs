using System.Collections.Immutable;
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
        public JsonConvertRepo(ImmutableArray<JsonConverter> converters)
        {
            Converters = converters;
        }

        /// <inheritdoc cref="IJsonConvertRepo.Converters"/>
        public ImmutableArray<JsonConverter> Converters { get; }
    }
}