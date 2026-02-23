namespace BoysheO.Extensions.Unity3D.Abstractions
{
    /// <summary>
    /// Exception thrown when a method is called from a thread other than the main thread.
    /// </summary>
    public sealed class NotMainThreadException:System.Exception
    {
        
    }
}