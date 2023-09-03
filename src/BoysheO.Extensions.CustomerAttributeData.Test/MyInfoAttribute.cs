namespace BoysheO.Extensions.CustomerAttributeData.Test;

public class MyInfoAttribute:Attribute
{
    public readonly string Str;

    public MyInfoAttribute(string str)
    {
        Str = str;
    }
}