using Hrx;

/// <summary>
/// 此事件源可能发出OnNext\OnCompete\OnError三种事件
/// </summary>
public interface INCEObservable<out T>:INCObservable<T>
{
}