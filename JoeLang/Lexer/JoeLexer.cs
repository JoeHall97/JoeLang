using JoeLang.Constants;
using JoeLang.Token;

namespace JoeLang.Lexer;

public class JoeLexer
{
    private readonly string input;
    // current position in the input (points to the current char)
    private int position;
    // current reading position in the input (points after the current char)
    private int readPosition;
    // current char
    private char ch;

    public JoeLexer(string input)
    {
        this.input = input;
        ReadChar();
    }

    public string Input
    {
        get { return input; }
    }

    public JoeToken NextToken()
    {
        JoeToken token;

        SkipWhitespace();

        switch (ch)
        {
            case '+':
                token = new JoeToken(TokenConstants.PLUS, ch.ToString());
                break;
            case '-':
                token = new JoeToken(TokenConstants.MINUS, ch.ToString());
                break;
            case '/':
                token = new JoeToken(TokenConstants.SLASH, ch.ToString());
                break;
            case '*':
                token = new JoeToken(TokenConstants.ASTERISK, ch.ToString());
                break;
            case '<':
                token = new JoeToken(TokenConstants.LT, ch.ToString());
                break;
            case '>':
                token = new JoeToken(TokenConstants.GT, ch.ToString());
                break;
            case ';':
                token = new JoeToken(TokenConstants.SEMICOLON, ch.ToString());
                break;
            case ',':
                token = new JoeToken(TokenConstants.COMMA, ch.ToString());
                break;
            case '(':
                token = new JoeToken(TokenConstants.LPAREN, ch.ToString());
                break;
            case ')':
                token = new JoeToken(TokenConstants.RPAREN, ch.ToString());
                break;
            case '{':
                token = new JoeToken(TokenConstants.LBRACE, ch.ToString());
                break;
            case '}':
                token = new JoeToken(TokenConstants.RBRACE, ch.ToString());
                break;
            case '\0':
                token = new JoeToken(TokenConstants.EOF, "");
                break;
            case '=':
                if (PeekChar() == '=')
                {
                    var ch = this.ch;
                    ReadChar();
                    var literal = ch.ToString() + this.ch.ToString();
                    token = new JoeToken(TokenConstants.EQ, literal);
                }
                else
                {
                    token = new JoeToken(TokenConstants.ASSIGN, ch.ToString());
                }
                break;
            case '!':
                if (PeekChar() == '=')
                {
                    var ch = this.ch;
                    ReadChar();
                    var literal = ch.ToString() + this.ch.ToString();
                    token = new JoeToken(TokenConstants.NOT_EQ, literal);
                }
                else
                {
                    token = new JoeToken(TokenConstants.BANG, ch.ToString());
                }
                break;
            default:
                if (char.IsLetter(ch))
                {
                    var literal = ReadIdentifier();
                    token = new JoeToken(Tokens.LookupIdentifier(literal), literal);
                    return token;
                }
                else if (char.IsDigit(ch))
                {
                    token = new JoeToken(TokenConstants.INT, ReadNumber());
                    return token;
                }
                else
                {
                    token = new JoeToken(TokenConstants.ILLEGAL, ch.ToString());
                }
                break;
        }

        ReadChar();
        return token;
    }

    
    public char PeekChar()
    {
        if (readPosition >= input.Length)
            return '\0';
        else
            return input[readPosition];
    }

    private void ReadChar()
    {
        if (readPosition >= input.Length)
            ch = '\0';
        else
            ch = input[readPosition];
        position = readPosition;
        readPosition += 1;
    }

    private void SkipWhitespace()
    {
        while (ch == ' ' || ch == '\t' || ch == '\n' || ch == '\r')
            ReadChar();
    }

    private string ReadIdentifier()
    {
        var pos = position;
        var len = 0;

        while (char.IsLetter(ch)) { ReadChar(); len++; }

        return input.Substring(pos, len);
    }

    private string ReadNumber()
    {
        var pos = position;
        var len = 0;

        while (char.IsDigit(ch)) { ReadChar(); len++; }

        return input.Substring(pos, len);
    }
}