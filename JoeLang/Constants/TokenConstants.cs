namespace JoeLang.Constants;

public static class TokenConstants
{
    public const string Illegal = "ILLEGAL";
    public const string Eof = "EOF";

    // Identifiers + literals
    public const string Ident = "IDENT";
    public const string Int = "INT";
    public const string String = "STRING";

    // Operators
    public const string Assign = "=";
    public const string Plus = "+";
    public const string Minus = "-";
    public const string Bang = "!";
    public const string Asterisk = "*";
    public const string Slash = "/";
    public const string Lt = "<";
    public const string Gt = ">";
    public const string Eq = "==";
    public const string NotEq = "!=";

    // Delimiters
    public const string Comma = ",";
    public const string Semicolon = ";";
    public const string Lparen = "(";
    public const string Rparen = ")";
    public const string Lbrace = "{";
    public const string Rbrace = "}";
    public const string Lbracket = "[";
    public const string Rbracket = "]";
    public const string Colon = ":";

    // Keywords
    public const string Function = "FUNCTION";
    public const string Let = "LET";
    public const string True = "TRUE";
    public const string False = "FALSE";
    public const string If = "IF";
    public const string Else = "ELSE";
    public const string Return = "RETURN";
}
