using JoeLang.Token;

namespace JoeLang.Lexer;

public class JoeLexer
{
    private string _input;
    // current position in the input (points to the current char)
    private int _position;
    // current reading position in the input (points after the current char)
    private int _readPosition;
    // current char
    private char _ch;

    public JoeLexer(string input)
    {
        _input = input;
        ReadChar();
    }

    public string Input
    {
        get { return _input; }
    }

    public JoeToken NextToken()
    {
        JoeToken token;

        SkipWhitespace();

        switch (_ch)
        {
            case '+':
                token = new JoeToken(Tokens.PLUS, _ch.ToString());
                break;
            case '-':
                token = new JoeToken(Tokens.MINUS, _ch.ToString());
                break;
            case '/':
                token = new JoeToken(Tokens.SLASH, _ch.ToString());
                break;
            case '*':
                token = new JoeToken(Tokens.ASTERISK, _ch.ToString());
                break;
            case '<':
                token = new JoeToken(Tokens.LT, _ch.ToString());
                break;
            case '>':
                token = new JoeToken(Tokens.GT, _ch.ToString());
                break;
            case ';':
                token = new JoeToken(Tokens.SEMICOLON, _ch.ToString());
                break;
            case ',':
                token = new JoeToken(Tokens.COMMA, _ch.ToString());
                break;
            case '(':
                token = new JoeToken(Tokens.LPAREN, _ch.ToString());
                break;
            case ')':
                token = new JoeToken(Tokens.RPAREN, _ch.ToString());
                break;
            case '{':
                token = new JoeToken(Tokens.LBRACE, _ch.ToString());
                break;
            case '}':
                token = new JoeToken(Tokens.RBRACE, _ch.ToString());
                break;
            case '\0':
                token = new JoeToken(Tokens.EOF, "");
                break;
            case '=':
                if (PeekChar() == '=')
                {
                    var ch = _ch;
                    ReadChar();
                    var literal = ch.ToString() + _ch.ToString();
                    token = new JoeToken(Tokens.EQ, literal);
                }
                else
                {
                    token = new JoeToken(Tokens.ASSIGN, _ch.ToString());
                }
                break;
            case '!':
                if (PeekChar() == '=')
                {
                    var ch = _ch;
                    ReadChar();
                    var literal = ch.ToString() + _ch.ToString();
                    token = new JoeToken(Tokens.NOT_EQ, literal);
                }
                else
                {
                    token = new JoeToken(Tokens.BANG, _ch.ToString());
                }
                break;
            default:
                if (char.IsLetter(_ch))
                {
                    var literal = ReadIdentifier();
                    token = new JoeToken(Tokens.LookupIdentifier(literal), literal);
                    return token;
                }
                else if (char.IsDigit(_ch))
                {
                    token = new JoeToken(Tokens.INT, ReadNumber());
                    return token;
                }
                else
                {
                    token = new JoeToken(Tokens.ILLEGAL, _ch.ToString());
                }
                break;
        }

        ReadChar();
        return token;
    }

    
    public char PeekChar()
    {
        if (_readPosition >= _input.Length)
            return '\0';
        else
            return _input[_readPosition];
    }

    private void ReadChar()
    {
        if (_readPosition >= _input.Length)
            _ch = '\0';
        else
            _ch = _input[_readPosition];
        _position = _readPosition;
        _readPosition += 1;
    }

    private void SkipWhitespace()
    {
        while (_ch == ' ' || _ch == '\t' || _ch == '\n' || _ch == '\r')
            ReadChar();
    }

    private string ReadIdentifier()
    {
        var pos = _position;
        var len = 0;

        while (char.IsLetter(_ch)) { ReadChar(); len++; }

        return _input.Substring(pos, len);
    }

    private string ReadNumber()
    {
        var pos = _position;
        var len = 0;

        while (char.IsDigit(_ch)) { ReadChar(); len++; }

        return _input.Substring(pos, len);
    }
}