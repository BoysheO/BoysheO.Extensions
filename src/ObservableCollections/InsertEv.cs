namespace ObservableCollections;

public readonly struct InsertEv<T>
{
    public readonly int Index;
    public readonly T Value;

    public InsertEv(int index, T value)
    {
        Index = index;
        Value = value;
    }
}