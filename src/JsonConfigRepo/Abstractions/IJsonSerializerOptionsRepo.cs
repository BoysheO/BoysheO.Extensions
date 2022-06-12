using System.Text.Json;

namespace JsonConfigRepo.Abstractions
{
    /// <summary>
    /// 提供5种基本预制，并且给出默认设定注释
    /// Provide 5 basic prefabs
    /// </summary>
    public interface IJsonSerializerOptionsRepo
    {
        /// <summary>
        ///     <para>标准序列化行为,添加对自定义类型的支持</para>
        ///     <para> follow the system default JsonSerializerOptions and support customer converter</para>
        /// </summary>
        JsonSerializerOptions StandardOptions { get; }

        /// <summary>
        ///     <para>Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping</para>
        ///     <para>添加对自定义类型的支持</para>
        /// </summary>
        JsonSerializerOptions UTF8Options { get; }

        /// <summary>
        ///     <para>WriteIndented = true,</para>
        ///     <para>其他同UTF8Options       </para>
        ///     <para>添加对自定义类型的支持          </para>
        /// </summary>
        JsonSerializerOptions UTF8PrettyOptions { get; }


        /// <summary>
        ///     <para>用于可视化对象的目的；</para>
        ///     <para>IncludeField =true,                                             </para>
        ///     <para>Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,          </para>
        ///     <para>DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,</para>
        /// </summary>
        /// <returns></returns>
        JsonSerializerOptions DebugJsonOptions { get; }

        /// <summary>
        ///     <para>用于可视化对象的目的；          </para>
        ///     <para>WriteIndented = true,</para>
        ///     <para>其他同DebugJsonOptions  </para>
        /// </summary>
        /// <returns></returns>
        JsonSerializerOptions DebugJsonPrettyOptions { get; }
    }
}