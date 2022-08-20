using System;
using System.Threading;
using UnityEngine;

namespace BoysheO.Extensions.Unity3D.Abstractions
{
    /// <summary>
    /// 
    /// </summary>
    public static class UnityEnvironmentExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="unityEnvironment"></param>
        /// <exception cref="Exception"></exception>
        public static void ThrowIfNotMainThread(this IUnityEnvironment unityEnvironment)
        {
            if (!unityEnvironment.IsInUnityThread()) throw new Exception("not in main thread");
        }
    }
}