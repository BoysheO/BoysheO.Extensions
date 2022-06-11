using System.Text.Json;

namespace JsonConfigRepo.Abstractions
{
    /// <summary>
    /// 提供5种基本预制，并且给出默认设定注释
    /// </summary>
    public interface IJsonSerializerOptionsRepo
    {
        /// <summary>
        ///     标准序列化行为
        ///     添加对自定义类型的支持
        /// </summary>
        JsonSerializerOptions StandardOptions { get; }

        /// <summary>
        ///     Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        ///     添加对自定义类型的支持
        /// </summary>
        JsonSerializerOptions UTF8Options { get; }

        /// <summary>
        ///     WriteIndented = true,
        ///     其他同UTF8Options
        ///     添加对自定义类型的支持
        /// </summary>
        JsonSerializerOptions UTF8PrettyOptions { get; }


        /// <summary>
        ///     用于可视化对象的目的；
        ///     IncludeField =true,
        ///     Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        ///     DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        /// </summary>
        /// <returns></returns>
        JsonSerializerOptions DebugJsonOptions { get; }

        /// <summary>
        ///     用于可视化对象的目的；
        ///     WriteIndented = true,
        ///     其他同DebugJsonOptions
        /// </summary>
        /// <returns></returns>
        JsonSerializerOptions DebugJsonPrettyOptions { get; }
    }
}