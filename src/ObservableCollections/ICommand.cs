namespace ObservableCollections;

public interface ICommand<TArg>
{
    event EventHandler CanExecuteChanged;
    bool CanExecute(TArg parameter);
    void StartExecute(TArg parameter,Action onDone,Action<Exception> onError);
}