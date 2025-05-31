using System;
using System.Threading;

namespace BoysheO.Extensions.Unity3D.Abstractions
{
    /// <summary>
    /// 提供Unity3D的环境(具体实现，在初始化后，应都支持多线程访问）
    /// </summary>
    public interface IUnityEnvironment
    {
        /// <summary>
        /// 等价<see cref="UnityEngine.Application.platform"/>，是它的字符串形式
        /// </summary>
        string Platform { get; }

        /// <summary>
        /// 是否播放中；此值应处处可用不抛异常，以解决Application.isPlaying在构造时抛异常的问题
        /// 在UnityEditor中，这个值应为开发者按下播放按钮之后会true，松开播放按钮之后为false。对应的值应为EditorApplication.isPlayingOrWillChangePlaymode（但是还是有点小区别）
        /// 在后端和Runtime中，这个值应恒为true。
        /// </summary>
        bool IsPlaying { get; }

        /// <summary>
        /// 等价<see cref="UnityEngine.Application.version"/>
        /// </summary>
        string Version { get; }

        /// <summary>
        /// 等价<see cref="UnityEngine.Application.streamingAssetsPath"/>
        /// </summary>
        string StreamingAssetsPath { get; }

        /// <summary>
        /// 等价<see cref="UnityEngine.Application.dataPath"/>
        /// </summary>
        string DataPath { get; }

        /// <summary>
        /// 等价<see cref="UnityEngine.Application.persistentDataPath"/>
        /// </summary>
        string PersistentDataPath { get; }

        /// <summary>
        /// 等价<see cref="UnityEngine.Application.temporaryCachePath"/>
        /// </summary>
        string TemporaryCachePath { get; }

        /// <summary>
        /// 等价<see cref="UnityEngine.Application.systemLanguage"/>，是它的字符串形式
        /// </summary>
        string SystemLanguage { get; }

        /// <summary>
        /// Unity3D主线程上下文
        /// </summary>
        SynchronizationContext SynchronizationContext { get; }

        /// <summary>
        /// Unity3D的线程引用
        /// </summary>
        Thread Thread { get; }

        /// <summary>
        /// 等价<see cref="UnityEngine.Application.productName"/>
        /// </summary>
        string ProductName { get; }

        /// <summary>
        /// 等价<see cref="UnityEngine.Application.quitting"/>
        /// </summary>
        event Action Quitting;

        /// <summary>
        /// 等价<see cref="UnityEngine.Application.wantsToQuit"/>
        /// </summary>
        event Func<bool> WantsToQuit;

        /// <summary>
        /// 当前上下文是否在Unity主线程内
        /// </summary>
        bool IsInUnityThread();

        /// <summary>
        /// 等价<see cref="UnityEngine.MonoBehaviour"/>.Update
        /// </summary>
        event Action onUpdate;

        /// <summary>
        /// 等价<see cref="UnityEngine.MonoBehaviour"/>.FixedUpdate
        /// </summary>
        event Action onFixedUpdate;

        /// <summary>
        /// 等价<see cref="UnityEngine.MonoBehaviour"/>.LateUpdate
        /// </summary>
        event Action onLateUpdate;
    }
}