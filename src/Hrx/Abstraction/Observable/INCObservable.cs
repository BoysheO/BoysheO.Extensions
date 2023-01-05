namespace Hrx
{
    /// <summary>
    /// 此事件源可能发出OnNext\OnCompete两种事件
    /// </summary>
    public interface INCObservable<out T>:INObservable<T>
    {
    }
}