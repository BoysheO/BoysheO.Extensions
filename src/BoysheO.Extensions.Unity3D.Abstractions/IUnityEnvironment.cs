using System;
using System.Threading;
using UnityEngine;

namespace BoysheO.Extensions.Unity3D.Abstractions
{
    /// <summary>
    /// 提供Unity3D的环境
    /// </summary>
    public interface IUnityEnvironment
    {
        /// <summary>
        ///     <see cref="Application.platform"/>
        /// </summary>
        RuntimePlatform Platform { get; }

        /// <summary>
        /// 根据IL2CPP运行环境的特点关闭一些Not-support特性，例如reload
        /// </summary>
        bool IsIL2CPP { get; }

        /// <summary>
        ///     等价或等义<see cref="Application.version"/>，支持多线程访问
        /// </summary>
        string Version { get; }

        /// <summary>
        ///     等价<see cref="Application.streamingAssetsPath"/>，支持多线程访问
        /// </summary>
        string StreamingAssetsPath { get; }

        /// <summary>
        ///     等价<see cref="Application.dataPath"/>，支持多线程访问
        /// </summary>
        string DataPath { get; }

        /// <summary>
        /// 等价<see cref="Application.persistentDataPath"/>，支持多线程访问
        /// </summary>
        string PersistentDataPath { get; }

        /// <summary>
        /// 等价<see cref="Application.temporaryCachePath"/>，支持多线程访问
        /// </summary>
        string TemporaryCachePath { get; }

        /// <summary>
        /// 等价<see cref="Application.systemLanguage"/>，支持多线程访问
        /// </summary>
        SystemLanguage SystemLanguage { get; }

        /// <summary>
        /// Unity3D主线程上下文
        /// </summary>
        SynchronizationContext SynchronizationContext { get; }

        /// <summary>
        /// Unity3D的线程引用
        /// </summary>
        Thread Thread { get; }

        /// <summary>
        /// 等价<see cref="Application.productName"/>，支持多线程访问
        /// </summary>
        string ProductName { get; }

        /// <summary>
        /// 等价<see cref="Application.quitting"/>
        /// </summary>
        event Action Quitting;

        /// <summary>
        /// 等价<see cref="Application.wantsToQuit"/>
        /// </summary>
        event Func<bool> WantsToQuit;

        /// <summary>
        /// 当前上下文是否在Unity主线程内
        /// </summary>
        bool IsInUnityThread();

        /// <summary>
        /// 等价<see cref="MonoBehaviour"/>.Update
        /// </summary>
        event Action onUpdate;

        /// <summary>
        /// 等价<see cref="MonoBehaviour"/>.FixedUpdate
        /// </summary>
        event Action onFixedUpdate;

        /// <summary>
        /// 等价<see cref="MonoBehaviour"/>.LateUpdate
        /// </summary>
        event Action onLateUpdate;
    }
}