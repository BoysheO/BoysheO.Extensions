namespace BoysheO.Extensions.Unity3D.Abstractions
{
    public static class UnityEnvironmentExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="unityEnvironment"></param>
        /// <exception cref="NotMainThreadException">NotInMainThread</exception>
        public static void ThrowIfNotMainThread(this IUnityEnvironment unityEnvironment)
        {
            if (!unityEnvironment.IsInUnityThread()) throw new NotMainThreadException();
        }
    }
}