// See https://aka.ms/new-console-template for more information

using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Resources.NetStandard;
using Microsoft.CSharp;

Console.WriteLine("Hello, World!");

args = new[]
{
    "E:\\temp\\runtime-8.05\\runtime-8.0.5\\src\\libraries\\System.Private.CoreLib\\src\\Resources\\Strings.resx",
    "E:\\a.cs"
};

//---------- 将resx文件转为C#代码
if (args.Length != 2)
{
    Console.WriteLine("Usage: ResxToConstants <input.resx> <output.cs>");
    return;
}

string resxFilePath = args[0];
string outputFilePath = args[1];

var resxEntries = new Dictionary<string, string>();

using (ResXResourceReader resxReader = new ResXResourceReader(resxFilePath))
{
    foreach (DictionaryEntry entry in resxReader)
    {
        string key = (string) entry.Key;
        string value = (string) entry.Value;
        resxEntries.Add(key, value);
    }
}

CodeCompileUnit compileUnit = new CodeCompileUnit();
CodeNamespace codeNamespace = new CodeNamespace("GeneratedConstants");
CodeTypeDeclaration classDeclaration = new CodeTypeDeclaration("ResxConstants")
{
    IsClass = true,
    TypeAttributes = System.Reflection.TypeAttributes.Public | System.Reflection.TypeAttributes.Sealed
};

foreach (var entry in resxEntries)
{
    CodeMemberField field = new CodeMemberField(typeof(string), entry.Key)
    {
        Attributes = MemberAttributes.Public | MemberAttributes.Static | MemberAttributes.Final,
        InitExpression = new CodePrimitiveExpression(entry.Value)
    };
    classDeclaration.Members.Add(field);
}

codeNamespace.Types.Add(classDeclaration);
compileUnit.Namespaces.Add(codeNamespace);

using (StreamWriter writer = new StreamWriter(outputFilePath, false))
{
    CodeDomProvider provider = new CSharpCodeProvider();
    provider.GenerateCodeFromCompileUnit(compileUnit, writer, new CodeGeneratorOptions());
}

Console.WriteLine($"Constants file generated: {outputFilePath}");