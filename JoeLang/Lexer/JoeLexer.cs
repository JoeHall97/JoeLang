using JoeLang.Constants;
using JoeLang.Token;

namespace JoeLang.Lexer;

public class JoeLexer
{
    // current position in the input (points to the current char)
    private int position;
    // current reading position in the input (points after the current char)
    private int readPosition;
    // current char
    private char ch;

    public JoeLexer(string input)
    {
        this.Input = input;
        ReadChar();
    }

    public string Input { get; }

    public JoeToken NextToken()
    {
        while (true)
        {
            JoeToken token;

            SkipWhitespace();

            switch (ch)
            {
                case '+':
                    token = new JoeToken(TokenConstants.Plus, ch.ToString());
                    break;
                case '-':
                    token = new JoeToken(TokenConstants.Minus, ch.ToString());
                    break;
                case '/':
                    if (PeekChar() != '/')
                    {
                        token = new JoeToken(TokenConstants.Slash, ch.ToString());
                        break;
                    }

                    SkipLine();
                    continue;
                case '*':
                    token = new JoeToken(TokenConstants.Asterisk, ch.ToString());
                    break;
                case '<':
                    token = new JoeToken(TokenConstants.Lt, ch.ToString());
                    break;
                case '>':
                    token = new JoeToken(TokenConstants.Gt, ch.ToString());
                    break;
                case ';':
                    token = new JoeToken(TokenConstants.Semicolon, ch.ToString());
                    break;
                case ':':
                    token = new JoeToken(TokenConstants.Colon, ch.ToString());
                    break;
                case ',':
                    token = new JoeToken(TokenConstants.Comma, ch.ToString());
                    break;
                case '(':
                    token = new JoeToken(TokenConstants.Lparen, ch.ToString());
                    break;
                case ')':
                    token = new JoeToken(TokenConstants.Rparen, ch.ToString());
                    break;
                case '{':
                    token = new JoeToken(TokenConstants.Lbrace, ch.ToString());
                    break;
                case '}':
                    token = new JoeToken(TokenConstants.Rbrace, ch.ToString());
                    break;
                case '[':
                    token = new JoeToken(TokenConstants.Lbracket, ch.ToString());
                    break;
                case ']':
                    token = new JoeToken(TokenConstants.Rbracket, ch.ToString());
                    break;
                case '\0':
                    token = new JoeToken(TokenConstants.Eof, "");
                    break;
                case '=':
                    if (PeekChar() == '=')
                    {
                        var ch = this.ch;
                        ReadChar();
                        var literal = ch.ToString() + this.ch.ToString();
                        token = new JoeToken(TokenConstants.Eq, literal);
                    }
                    else
                    {
                        token = new JoeToken(TokenConstants.Assign, ch.ToString());
                    }

                    break;
                case '!':
                    if (PeekChar() == '=')
                    {
                        var ch = this.ch;
                        ReadChar();
                        var literal = ch.ToString() + this.ch.ToString();
                        token = new JoeToken(TokenConstants.NotEq, literal);
                    }
                    else
                    {
                        token = new JoeToken(TokenConstants.Bang, ch.ToString());
                    }

                    break;
                case '"':
                    token = new JoeToken(TokenConstants.String, ReadString());
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
                        token = new JoeToken(TokenConstants.Int, ReadNumber());
                        return token;
                    }
                    else
                    {
                        token = new JoeToken(TokenConstants.Illegal, ch.ToString());
                    }

                    break;
            }

            ReadChar();
            return token;
        }
    }


    public char PeekChar()
    {
        return readPosition >= Input.Length ? '\0' : Input[readPosition];
    }

    public string ReadString()
    {
        var pos = position + 1;

        while (true) 
        { 
            ReadChar();
            if (ch == '"' || ch == '\0')
                break;
        }

        return Input.Substring(pos,position-pos);
    }

    private void ReadChar()
    {
        ch = readPosition >= Input.Length ? '\0' : Input[readPosition];
        position = readPosition;
        readPosition += 1;
    }

    private void SkipWhitespace()
    {
        while (ch is ' ' or '\t' or '\n' or '\r')
            ReadChar();
    }

    private void SkipLine()
    {
        while (ch != '\n' && ch != '\r' && ch != '\0')
            ReadChar();
    }

    private string ReadIdentifier()
    {
        var pos = position;
        var len = 0;

        while (char.IsLetter(ch)) { ReadChar(); len++; }

        return Input.Substring(pos, len);
    }

    private string ReadNumber()
    {
        var pos = position;
        var len = 0;

        while (char.IsDigit(ch)) { ReadChar(); len++; }

        return Input.Substring(pos, len);
    }
}