using JoeLang.Constants;

namespace JoeLang.Token;

public struct JoeToken
{
    public JoeToken(string type, string literal)
    {
        Type = type;
        Literal = literal;
    }

    public string Type;
    public string Literal;
}

public static class Tokens 
{
    private static Dictionary<string, string> _keywordsMap = new()
    {
        {"fn" , TokenConstants.FUNCTION},
        {"var" , TokenConstants.VAR},
        {"true" , TokenConstants.TRUE},
        {"false" , TokenConstants.FALSE},
        {"if" , TokenConstants.IF},
        {"else" , TokenConstants.ELSE},
        {"return" , TokenConstants.RETURN},
    };

    public static string LookupIdentifier(string ident)
    {
        if (_keywordsMap.ContainsKey(ident))
            return _keywordsMap[ident];

        return TokenConstants.IDENT;
    }

    public static JoeToken NewToken(string type, string literal)
    {
        return new JoeToken(type, literal);
    }
}