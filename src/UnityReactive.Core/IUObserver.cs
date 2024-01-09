namespace UnityReactive.Core
{
    public interface IUObserver
    {
        void OnDead();
    }
    
    public interface IUObserver<in T>:IUObserver
    {
        void OnNext(T value);
    }
}