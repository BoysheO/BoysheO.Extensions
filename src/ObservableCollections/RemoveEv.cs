namespace ObservableCollections;

public readonly struct RemoveEv<T>
{
    public readonly int Index;
    public readonly T Value;

    public RemoveEv(int index, T value)
    {
        Index = index;
        Value = value;
    }
}