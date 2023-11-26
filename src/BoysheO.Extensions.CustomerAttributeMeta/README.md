## **Import
Actually,I feel this library is not easy to use.Maybe this idea is not good enough.So,I will not update the library until I have a better idea.

# BoysheO.Extensions.CustomerAttributeMeta

## what is this  
Unity3D with a special il2cpp version(hybridclr),carsh with using the API 'GetCustomerAttribute',so I creat this library for giving a way to get CustomerAttribute not too hard.

## How to use

The original code:
```csharp
[MyInfo("hello")]
public class Tests{    }
```
Append the meta code:
```csharp
[MyInfo("hello")]
public class Tests
{
    [UnityEngine.Scripting.Preserve]
    private class Tests_AtbMeta : ICustomerAttributeMeta
    {
        public Type Owner => typeof(Tests);
        public Attribute? CustomerAttribute { get; } = new MyInfoAttribute("hello");

        public Attribute[] CustomerAttributes { get; } = null;
    }
}
```
* You can put the XXX_AtbMeta anywhere,but the best way is 👆.
* Don't forget mark it [UnityEngine.Scripting.Preserve].

Now,you can use GetCustomerAttributeMeta to get customer attribute.
```csharp
var atb = type.GetCustomAttribute<MyInfoAttribute>() ?? throw new Exception();//crack in (hybridclr)
var atb2 = type.GetCustomerAttributeMeta<MyInfoAttribute>(false) ?? throw new Exception();//ok
```
 
## Limit 
It's only support type data.I don't have time to make it perfect now.See road map.

## Road Map  
* Make the inherit parma working more prefect.
* Provide a Analyser package for generate the meta code automatically.
* Support MemberInfo instead type 

