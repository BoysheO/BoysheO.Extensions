// ReSharper disable All
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace BoysheO.Extensions.Unity3D.Abstractions
{
    /// <summary>
    ///  UnityEngine.Time 的接口。
    /// </summary>
    public interface IUnityTime
    {
        float time { get; }
        double timeAsDouble { get; }
        float timeSinceLevelLoad { get; }
        float deltaTime { get; }
        float fixedTime { get; }
        float fixedUnscaledTime { get; }
        float fixedDeltaTime { get; set; }
        float maximumDeltaTime { get; set; }
        float smoothDeltaTime { get; }
        float maximumParticleDeltaTime { get; set; }
        float timeScale { get; set; }
        int frameCount { get; }
        int renderFrameCount { get; }
        float realtimeSinceStartup { get; }
        float unscaledTime { get; }
        float unscaledDeltaTime { get; }
        double fixedTimeAsDouble { get; }
        double unscaledTimeAsDouble { get; }
        double realtimeSinceStartupAsDouble { get; }
        float captureDeltaTime { get; set; }
        int captureFramerate { get; set; }
        bool inFixedTimeStep { get; }
    }
}