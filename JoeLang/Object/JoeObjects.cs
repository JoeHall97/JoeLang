using JoeLang.Constants;
using System.Text;

namespace JoeLang.Object;

public interface IJoeObject
{
    string Type();
    string Inspect();
}

public delegate IJoeObject BuiltinFunction(params IJoeObject[] args);

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
    public string Inspect() { return $"[{string.Join(", ", elements.Select(e => e.Inspect()))}]"; }
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
}

public class JoeInteger :IJoeObject
{ 
    private readonly long value;

    public JoeInteger(long value) { this.value = value; }

    public long Value { get => value; }

    public string Type() { return ObjectConstants.INT_OBJECT; }

    public string Inspect() { return value.ToString(); }
}

public class JoeBoolean : IJoeObject
{
    private readonly bool value;

    public JoeBoolean(bool value) { this.value = value; }

    public bool Value { get => value; }

    public string Type() { return ObjectConstants.BOOL_OBJECT; }

    public string Inspect() { return value.ToString(); }
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

    public JoeFunction(AST.Identifier[] parameters, AST.BlockStatement body, JoeEnvironment environment)
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
        return $"fn({string.Join(',', parameters.Select(p => p.ToString()))}) {{\n{body}\n}}";
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