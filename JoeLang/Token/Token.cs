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
    public const string ILLEGAL = "ILLEGAL";
    public const string EOF     = "EOF";

    // Identifiers + literals
    public const string IDENT = "IDENT";
    public const string INT   = "INT";

    // Operators
    public const string ASSIGN   = "=";
	public const string PLUS     = "+";
	public const string MINUS    = "-";
	public const string BANG     = "!";
	public const string ASTERISK = "*";
	public const string SLASH    = "/";
	public const string LT       = "<";
	public const string GT       = ">";
	public const string EQ       = "==";
	public const string NOT_EQ   = "!=";

    // Delimiters
    public const string COMMA     = ",";
	public const string SEMICOLON = ";";
	public const string LPAREN    = "(";
	public const string RPAREN    = ")";
	public const string LBRACE    = "{";
	public const string RBRACE    = "}";

    // Keywords
	public const string FUNCTION = "FUNCTION";
	public const string VAR      = "VAR";
	public const string TRUE     = "TRUE";
	public const string FALSE    = "FALSE";
	public const string IF       = "IF";
	public const string ELSE     = "ELSE";
	public const string RETURN   = "RETURN";

    private static Dictionary<string, string> _keywordsMap = new()
    {
        {"fn" , FUNCTION},
        {"var" , VAR},
        {"true" , TRUE},
        {"false" , FALSE},
        {"if" , IF},
        {"else" , ELSE},
        {"return" , RETURN},
    };

    public static string LookupIdentifier(string ident)
    {
        if (_keywordsMap.ContainsKey(ident))
            return _keywordsMap[ident];

        return IDENT;
    }

    public static JoeToken NewToken(string type, string literal)
    {
        return new JoeToken(type, literal);
    }
}