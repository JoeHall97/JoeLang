using JoeLang.AST;
using JoeLang.Lexer;
using JoeLang.Token;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace JoeLang.Parser;

public class DescentParser
{
    public delegate IExpressionNode? PrefixParseFunction();
    public delegate IExpressionNode? InfixParseFunction(IExpressionNode expressionNode);

    public enum Precedence
    {
        LOWEST,
        EQUALS,
        LESSGREATER,
        SUM,
        PRODUCT,
        PREFIX,
        CALL
    }

    private readonly JoeLexer _lexer;

    private JoeToken _currToken;
    private JoeToken _peekToken;
    private List<string> _errors;

    private Dictionary<string, PrefixParseFunction> _prefixFunctions;
    private Dictionary<string, InfixParseFunction> _infixFunctions;

    private readonly Dictionary<string, Precedence> _precidenceMap = new()
    {
        { Tokens.EQ, Precedence.EQUALS },
        { Tokens.NOT_EQ, Precedence.EQUALS },
        { Tokens.LT, Precedence.LESSGREATER },
        { Tokens.GT, Precedence.LESSGREATER },
        { Tokens.PLUS, Precedence.SUM },
        { Tokens.MINUS, Precedence.SUM },
        { Tokens.SLASH, Precedence.PRODUCT },
        { Tokens.ASTERISK, Precedence.PRODUCT },
        { Tokens.LPAREN, Precedence.CALL },
    };
    
    public DescentParser(JoeLexer lexer) 
    {
        _lexer = lexer;
        _errors = new List<string>();

        // read in two tokens to make sure that both curr and peek tokens are set
        NextToken();
        NextToken();

        _prefixFunctions = new()
        {
            { Tokens.IDENT, ParseIdentifier },
            { Tokens.INT, ParseIntegerLiteral },
            { Tokens.MINUS, ParsePrefixExpression },
            { Tokens.BANG, ParsePrefixExpression },
            { Tokens.TRUE, ParseBoolean },
            { Tokens.FALSE, ParseBoolean },
            { Tokens.LPAREN, ParseGroupExpression },
            { Tokens.IF, ParseIfExpression },
            { Tokens.FUNCTION, ParseFunctionLiteral },
        };

        _infixFunctions = new()
        {

            { Tokens.PLUS, ParseInfixExpression },
            { Tokens.MINUS, ParseInfixExpression },
            { Tokens.SLASH, ParseInfixExpression },
            { Tokens.ASTERISK, ParseInfixExpression },
            { Tokens.EQ, ParseInfixExpression },
            { Tokens.NOT_EQ, ParseInfixExpression },
            { Tokens.LT, ParseInfixExpression },
            { Tokens.GT, ParseInfixExpression },
            { Tokens.LPAREN, ParseCallExpression },
        };
    }

    public string[] Errors
    {
        get => _errors.ToArray();
    }

    public JoeProgram ParseProgram()
    {
        List<IStatementNode> statements = new();

        while (!CurrentTokenIs(Tokens.EOF)) 
        {
            var statement = ParseStatement();
            if (statement != null)
                statements.Add(statement);
            NextToken();
        }

        return new JoeProgram(statements.ToArray());
    }

    private void NextToken()
    {
        _currToken = _peekToken;
        _peekToken = _lexer.NextToken();
    }

    private bool CurrentTokenIs(string tokenType)
    {
        return string.Equals(_currToken.Type, tokenType);
    }

    private bool PeekTokenIs(string tokenType)
    {
        return string.Equals(tokenType, _peekToken.Type);
    }

    private Precedence PeekPrecedence()
    {
        if (_precidenceMap.TryGetValue(_peekToken.Type, out Precedence precedence))
            return precedence;
        return Precedence.LOWEST;
    }

    private Precedence CurrentPrecedence()
    {
        if (_precidenceMap.TryGetValue(_currToken.Type, out Precedence currentPrecedence))
            return currentPrecedence;
        return Precedence.LOWEST;
    }

    private bool PeekExpected(string tokenType)
    {
        if (PeekTokenIs(tokenType))
        {
            NextToken();
            return true;
        }
        
        _errors.Add($"expected next token to be {tokenType}, got {_peekToken.Type} instead");
        return false;
    }

    private void NoPrefixParseFunctionError(string tokenType)
    {
        _errors.Add($"no prefix parse function for {tokenType} found");
    }

    private IExpressionNode? ParseExpression(Precedence precidence)
    {
        PrefixParseFunction prefixParseFunction;
        var foundParseFunction = _prefixFunctions.TryGetValue(_currToken.Type, out prefixParseFunction);
        if (!foundParseFunction) 
        {
            NoPrefixParseFunctionError(_currToken.Type);
            return null;
        }
        var leftExpression = prefixParseFunction();

        while (!PeekTokenIs(Tokens.SEMICOLON) && precidence < PeekPrecedence())
        {
            var infixFunction = _infixFunctions[_peekToken.Type];
            if (infixFunction == null)
                return leftExpression;

            NextToken();

            leftExpression = infixFunction(leftExpression);
        }

        return leftExpression;
    }

    private IExpressionNode? ParseIdentifier()
    {
        return new Identifier(_currToken, _currToken.Literal);
    }

    private IExpressionNode? ParseIntegerLiteral()
    {
        if (!long.TryParse(_currToken.Literal, out long value))
        {
            var errorMessage = $"could not parse {_currToken.Literal} as integer";
            _errors.Add(errorMessage);
            return null;
        }

        return new IntegerLiteral(_currToken, value);
    }

    private IExpressionNode? ParsePrefixExpression()
    {
        var currToken = _currToken; 
        var currTokenLiteral = _currToken.Literal;

        NextToken();

        var right = ParseExpression(Precedence.PREFIX);

        return new PrefixExpression(currToken, currTokenLiteral, right);
    }

    private IExpressionNode? ParseBoolean()
    {
        return new AST.Boolean(_currToken, CurrentTokenIs(Tokens.TRUE));
    }

    private IExpressionNode? ParseGroupExpression()
    {
        NextToken();

        var expression = ParseExpression(Precedence.LOWEST);
        if (PeekExpected(Tokens.RPAREN))
            return null;

        return expression;
    }

    private VarStatement? ParseVarStatement()
    {
        JoeToken currToken = _currToken;
        Identifier name;
        IExpressionNode? value;

        if (!PeekExpected(Tokens.IDENT))
            return null;

        name = new Identifier(_currToken, _currToken.Literal);

        if (!PeekExpected(Tokens.ASSIGN)) 
            return null;

        NextToken();

        value = ParseExpression(Precedence.LOWEST);

        if (PeekTokenIs(Tokens.SEMICOLON))
            NextToken();

        return new VarStatement(currToken, name, value);
    }

    private ReturnStatement? ParseReturnStatement()
    {
        JoeToken currToken = _currToken;
        IExpressionNode? returnValue;

        NextToken();

        returnValue = ParseExpression(Precedence.LOWEST);

        if (PeekExpected(Tokens.SEMICOLON))
            NextToken();

        return new ReturnStatement(currToken, returnValue);
    }

    private ExpressionStatement? ParseExpressionStatement()
    {
        JoeToken currToken = _currToken;
        IExpressionNode? expression = ParseExpression(Precedence.LOWEST);

        if (PeekTokenIs(Tokens.SEMICOLON))
            NextToken();

        return new ExpressionStatement(currToken, expression);
    }

    private IStatementNode? ParseStatement()
    {
        switch (_currToken.Type) 
        {
            case Tokens.VAR:
                return ParseVarStatement();
            case Tokens.RETURN:
                return ParseReturnStatement();
            default:
                return ParseExpressionStatement();
        }
    }

    private BlockStatement ParseBlockStatement()
    {
        JoeToken currToken = _currToken;
        List<IStatementNode> statements = new List<IStatementNode>();

        NextToken();

        while (!CurrentTokenIs(Tokens.RBRACE) && !CurrentTokenIs(Tokens.EOF)) 
        {
            var statement = ParseStatement();
            if (statement != null)
                statements.Add(statement);
            NextToken();
        }

        return new BlockStatement(_currToken, statements.ToArray());
    }

    private IExpressionNode? ParseIfExpression()
    {
        JoeToken currToken = _currToken;

        IExpressionNode condition;
        BlockStatement consequence;
        BlockStatement? alternative = null;

        if (!PeekExpected(Tokens.LPAREN))
            return null;

        NextToken();
        condition = ParseExpression(Precedence.LOWEST);

        if (!PeekExpected(Tokens.RPAREN))
            return null;

        if (!PeekExpected(Tokens.LBRACE))
            return null;

        consequence = ParseBlockStatement();

        if (PeekTokenIs(Tokens.ELSE))
        {
            NextToken();

            if (!PeekExpected(Tokens.LBRACE))
                return null;

            alternative = ParseBlockStatement();
        }

        return new IfExpression(currToken, condition, consequence, alternative);
    }

    private Identifier[]? ParseFunctionParameters()
    {
        List<Identifier> identifiers = new List<Identifier>();

        if (PeekTokenIs(Tokens.RPAREN))
        {
            NextToken();
            return identifiers.ToArray();
        }

        NextToken();

        var identifier = new Identifier(_currToken, _currToken.Literal);
        identifiers.Add(identifier);

        while (PeekTokenIs(Tokens.COMMA))
        {
            NextToken();
            NextToken();
            identifier = new Identifier(_currToken, _currToken.Literal);
            identifiers.Add(identifier);
        }

        if (!PeekExpected(Tokens.RPAREN))
            return null;

        return identifiers.ToArray();
    }

    private IExpressionNode? ParseFunctionLiteral()
    {
        JoeToken currToken = _currToken;
        Identifier[]? functionParamaters;
        BlockStatement body;

        if (!PeekExpected(Tokens.LPAREN))
            return null;

        functionParamaters = ParseFunctionParameters();

        if (!PeekExpected(Tokens.LBRACE))
            return null;

        body = ParseBlockStatement();

        return new FunctionLiteral(currToken, functionParamaters, body);
    }

    private InfixExpression ParseInfixExpression(IExpressionNode left)
    {
        JoeToken currToken = _currToken;
        string op = _currToken.Literal;
        IExpressionNode? right;

        var precedence = CurrentPrecedence();
        NextToken();
        right = ParseExpression(precedence);

        return new InfixExpression(currToken, left, op, right);
    }

    private IExpressionNode[]? ParseCallArguments()
    {
        List<IExpressionNode> arguments = new List<IExpressionNode>();

        if (PeekTokenIs(Tokens.RPAREN))
        {
            NextToken();
            return arguments.ToArray();
        }

        NextToken();

        arguments.Add(ParseExpression(Precedence.LOWEST));

        while (PeekTokenIs(Tokens.COMMA)) 
        { 
            NextToken();
            NextToken();

            arguments.Add(ParseExpression(Precedence.LOWEST));
        }

        if (!PeekExpected(Tokens.RPAREN))
            return null;

        return arguments.ToArray();
    }

    private CallExpression ParseCallExpression(IExpressionNode function)
    {
        JoeToken currToken = _currToken;
        IExpressionNode[]? arguments = ParseCallArguments();
        return new CallExpression(currToken, function, arguments);
    }
}
