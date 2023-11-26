using JoeLang.Constants;
using JoeLang.Object;

namespace JoeLang.Evaluator;

public static class Builtins
{
    public static readonly Dictionary<string, JoeBuiltin> builtins = new()
    {
        { "len", new JoeBuiltin(LenBuiltin) },
        { "first", new JoeBuiltin(FirstBuiltin) },
        { "last", new JoeBuiltin(LastBuiltin) },
        { "rest", new JoeBuiltin(RestBuiltin) },
        { "push", new JoeBuiltin(PushBuiltin) },
    };

    private static IJoeObject LenBuiltin(params IJoeObject[] args)
    {
        if (args.Length != 1)
            return new JoeError($"wrong number of arguments. got={args.Length}, want=1");

        return args[0] switch
        {
            JoeString joeString => new JoeInteger(joeString.Value.Length),
            JoeArray joeArray => new JoeInteger(joeArray.Elements.Length),
            _ => new JoeError($"argument to 'len' not supported. got={args[0].Type()}")
        };
    }

    private static IJoeObject FirstBuiltin(params IJoeObject[] args) 
    {
        if (args.Length != 1)
            return new JoeError($"wrong number of arguments. got={args.Length}, want=1");

        if (args[0] is JoeArray array)
            return array.Elements.Length > 0 ? array.Elements[0] : EvaluatorConstants.NULL;

        return new JoeError($"argument to 'first' must be an ARRAY. got={args[0].Type()}");
    }

    private static IJoeObject LastBuiltin(params IJoeObject[] args)
    {
        if (args.Length != 1)
            return new JoeError($"wrong number of arguments. got={args.Length}, want=1");

        if (args[0] is not JoeArray)
            return new JoeError($"argument to 'last' must be an ARRAY. got={args[0].Type()}");

        var array = (JoeArray)args[0];
        var length = array.Elements.Length;
        return length > 0 ? array.Elements[length - 1] : EvaluatorConstants.NULL;
    }

    private static IJoeObject RestBuiltin(params IJoeObject[] args)
    {
        if (args.Length != 1)
            return new JoeError($"wrong number of arguments. got={args.Length}, want=1");

        if (args[0] is not JoeArray)
            return new JoeError($"argument to 'rest' must be an ARRAY. got={args[0].Type()}");

        var array = (JoeArray)args[0];
        var length = array.Elements.Length;
        if (length > 0)
        {
            var newElements = new IJoeObject[length-1];
            array.Elements.Skip(1).ToArray().CopyTo(newElements, 0);
            return new JoeArray(newElements);
        }
        return EvaluatorConstants.NULL;
    }

    private static IJoeObject PushBuiltin(params IJoeObject[] args)
    {
        if (args.Length != 2)
            return new JoeError($"wrong number of arguments. got={args.Length}, want=2");

        if (args[0] is not JoeArray)
            return new JoeError($"argument to 'push' must be an ARRAY. got={args[0].Type()}");

        var array = (JoeArray)args[0];
        var length = array.Elements.Length;

        var newElements = new IJoeObject[length + 1];
        for (var i = 0; i < length; i++)
            newElements[i] = array.Elements[i];
        newElements[length] = args[1];

        return new JoeArray(newElements);
    }
}
