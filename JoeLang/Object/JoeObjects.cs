using JoeLang.Constants;
using System.Security.Cryptography;
using System.Text;

namespace JoeLang.Object;

public interface IJoeObject
{
    string Type();
    string Inspect();
}

public interface IHashable
{
    HashKey HashKey();
}

public delegate IJoeObject BuiltinFunction(params IJoeObject[] args);

public struct HashKey
{
    public readonly string objectType;
    public readonly long value;

    public HashKey(string objectType, long value)
    {
        this.objectType = objectType;
        this.value = value;
    }
}

public struct HashPair
{
    public readonly IJoeObject key;
    public readonly IJoeObject value;

    public HashPair(IJoeObject key, IJoeObject value)
    {
        this.key = key;
        this.value = value;
    }
}

public class JoeHash : IJoeObject
{
    private Dictionary<HashKey,HashPair> pairs;

    public JoeHash(Dictionary<HashKey, HashPair> pairs)
    {
        this.pairs = pairs;
    }

    public Dictionary<HashKey,HashPair> Pairs { get => pairs; }

    public string Type() { return ObjectConstants.HASH_OBJECT; }
    public string Inspect() 
    { 
        StringBuilder sb = new();

        sb.Append('{');
        foreach (var p in pairs.Values) 
            sb.Append($"{p.key.Inspect()}: {p.value.Inspect()}");
        sb.Append('}');

        return sb.ToString();
    }
}


public class JoeBuiltin : IJoeObject
{
    private readonly BuiltinFunction function;

    public JoeBuiltin(BuiltinFunction function)
    {
        this.function = function;
    }

    public BuiltinFunction Function { get => function; }

    public string Type() { return ObjectConstants.BUILTIN_OBJECT; }
    public string Inspect() { return "builtin function"; }
}

public class JoeArray : IJoeObject
{
    private readonly IJoeObject[] elements;

    public JoeArray(IJoeObject[] elements)
    {
        this.elements = elements;
    }

    public IJoeObject[] Elements { get => elements; }

    public string Type() { return ObjectConstants.ARRAY_OBJECT; }
    public string Inspect() 
    { 
        return $"[{string.Join(", ", elements.Select(e => e.Inspect()))}]"; 
    }
}

public class JoeString : IJoeObject
{
    private readonly string value;

    public JoeString(string value)
    {
        this.value = value;
    }

    public string Value { get => value; }

    public string Type() { return ObjectConstants.STRING_OBJECT; }
    public string Inspect() { return value; }
    public HashKey HashKey()
    {
        var hash = SHA1.HashData(Encoding.ASCII.GetBytes(value));
        long hashValue = 0;
        foreach (var h in hash)
            hashValue += h;
        return new HashKey(Type(), hashValue);
    }
}

public class JoeInteger :IJoeObject, IHashable
{ 
    private readonly long value;

    public JoeInteger(long value) { this.value = value; }

    public long Value { get => value; }

    public string Type() { return ObjectConstants.INT_OBJECT; }
    public string Inspect() { return value.ToString(); }
    public HashKey HashKey() { return new HashKey(Type(), value); }
}

public class JoeBoolean : IJoeObject, IHashable
{
    private readonly bool value;

    public JoeBoolean(bool value) { this.value = value; }

    public bool Value { get => value; }

    public string Type() { return ObjectConstants.BOOL_OBJECT; }
    public string Inspect() { return value.ToString(); }
    public HashKey HashKey() { return new HashKey(Type(), value ? 1 : 0); }
}

public class JoeReturnValue : IJoeObject
{
    private readonly IJoeObject value;

    public JoeReturnValue(IJoeObject value) { this.value = value; }

    public IJoeObject Value { get => value; }

    public string Type() { return ObjectConstants.RETURN_OBJECT; }

    public string Inspect() { return value.Inspect(); }
}

public class JoeFunction : IJoeObject
{
    private readonly AST.Identifier[] parameters;
    private readonly AST.BlockStatement body;
    private readonly JoeEnvironment environment;

    public JoeFunction(AST.Identifier[] parameters, AST.BlockStatement body, 
        JoeEnvironment environment)
    {
        this.parameters = parameters;
        this.body = body;
        this.environment = environment;
    }

    public AST.Identifier[] Parameters { get => parameters; }
    public AST.BlockStatement Body { get => body; }
    public JoeEnvironment Environment { get => environment; }

    public string Type() { return ObjectConstants.FUNCTION_OBJECT; }

    public string Inspect()
    {
        StringBuilder sb = new();

        sb.Append("fn(");
        sb.Append(string.Join(',', parameters.Select(p => p.ToString())));
        sb.Append(") {\n");
        sb.Append(body);
        sb.Append("\n}");

        return sb.ToString();
    }
}

public class JoeNull : IJoeObject
{
    public JoeNull() { }

    public string Type() { return ObjectConstants.NULL_OBJECT; }

    public string Inspect() { return "null"; }
}

public class JoeError : IJoeObject
{
    private readonly string message;

    public JoeError(string message) { this.message = message; }

    public string Message { get => message; }

    public string Type() { return ObjectConstants.ERROR_OBJECT; }

    public string Inspect() { return $"ERROR: {message}"; }
}