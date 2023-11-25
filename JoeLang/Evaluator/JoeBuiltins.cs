using JoeLang.Object;

namespace JoeLang.Evaluator;

public static class Builtins
{
    public static readonly Dictionary<string, JoeBuiltin> builtins = new()
    {
        { "len", new JoeBuiltin(LenBuiltin) }
    };

    private static IJoeObject LenBuiltin(params IJoeObject[] args)
    {
        if (args.Length != 1)
            return new JoeError($"wrong number of arguments. got={args.Length}, want=1");

        return args[0] switch
        {
            JoeString joeString => new JoeInteger(joeString.Value.Length),
            _ => new JoeError($"argument to 'len' not supported. got={args[0].Type()}")
        };
    }
}
