using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace JsonConfigRepo.Abstractions
{
    /// <summary>
    /// 提供Json序列化库注册
    /// </summary>
    public interface IJsonConvertRepo
    {
        /// <summary>
        /// 所有需要注册到Json序列化器的Converter
        /// </summary>
        IReadOnlyCollection<JsonConverter> Converters { get; }
    }
}