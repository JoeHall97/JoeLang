﻿using JoeLang.AST;
using JoeLang.Constants;
using JoeLang.Lexer;
using JoeLang.Token;

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
        CALL,
        INDEX
    }

    private readonly JoeLexer lexer;

    private JoeToken currToken;
    private JoeToken peekToken;
    private List<string> errors;

    private readonly Dictionary<string, PrefixParseFunction> prefixFunctions;
    private readonly Dictionary<string, InfixParseFunction> infixFunctions;

    private readonly Dictionary<string, Precedence> precidenceMap = new()
    {
        { TokenConstants.Eq, Precedence.EQUALS },
        { TokenConstants.NotEq, Precedence.EQUALS },
        { TokenConstants.Lt, Precedence.LESSGREATER },
        { TokenConstants.Gt, Precedence.LESSGREATER },
        { TokenConstants.Plus, Precedence.SUM },
        { TokenConstants.Minus, Precedence.SUM },
        { TokenConstants.Slash, Precedence.PRODUCT },
        { TokenConstants.Asterisk, Precedence.PRODUCT },
        { TokenConstants.Lparen, Precedence.CALL },
        { TokenConstants.Lbracket, Precedence.INDEX },
    };
    
    public DescentParser(JoeLexer lexer) 
    {
        this.lexer = lexer;
        errors = new List<string>();

        // read in two tokens to make sure that both curr and peek tokens are set
        NextToken();
        NextToken();

        prefixFunctions = new()
        {
            { TokenConstants.Ident, ParseIdentifier },
            { TokenConstants.Int, ParseIntegerLiteral },
            { TokenConstants.Minus, ParsePrefixExpression },
            { TokenConstants.Bang, ParsePrefixExpression },
            { TokenConstants.True, ParseBoolean },
            { TokenConstants.False, ParseBoolean },
            { TokenConstants.Lparen, ParseGroupExpression },
            { TokenConstants.If, ParseIfExpression },
            { TokenConstants.Function, ParseFunctionLiteral },
            { TokenConstants.String, ParseStringLiteral },
            { TokenConstants.Lbracket, ParseArrayLiteral },
            { TokenConstants.Lbrace, ParseHashLiteral }
        };

        infixFunctions = new()
        {
            { TokenConstants.Plus, ParseInfixExpression },
            { TokenConstants.Minus, ParseInfixExpression },
            { TokenConstants.Slash, ParseInfixExpression },
            { TokenConstants.Asterisk, ParseInfixExpression },
            { TokenConstants.Eq, ParseInfixExpression },
            { TokenConstants.NotEq, ParseInfixExpression },
            { TokenConstants.Lt, ParseInfixExpression },
            { TokenConstants.Gt, ParseInfixExpression },
            { TokenConstants.Lparen, ParseCallExpression },
            { TokenConstants.Lbracket, ParseIndexExpression },
        };
    }

    public string[] Errors
    {
        get => errors.ToArray();
    }

    public JoeProgram ParseProgram()
    {
        List<IStatementNode> statements = new();

        while (!CurrentTokenIs(TokenConstants.Eof)) 
        {
            var statement = ParseStatement();
            if (statement != null)
                statements.Add(statement);
            NextToken();
        }

        return new JoeProgram(statements.ToArray());
    }

    private IExpressionNode? ParseHashLiteral()
    {
        var token = currToken;
        var pairs = new Dictionary<IExpressionNode, IExpressionNode>();

        while (!PeekTokenIs(TokenConstants.Rbrace))
        {
            NextToken();
            var key = ParseExpression(Precedence.LOWEST);

            if (!PeekExpected(TokenConstants.Colon))
                return null;

            NextToken();
            var value = ParseExpression(Precedence.LOWEST);

            pairs[key] = value;

            if (!PeekTokenIs(TokenConstants.Rbrace) && !PeekExpected(TokenConstants.Comma))
                return null;
        }

        return PeekExpected(TokenConstants.Rbrace) ? new HashLiteral(token, pairs) : null;
    }

    private void NextToken()
    {
        currToken = peekToken;
        peekToken = lexer.NextToken();
    }

    private bool CurrentTokenIs(string tokenType)
    {
        return string.Equals(currToken.Type, tokenType);
    }

    private bool PeekTokenIs(string tokenType)
    {
        return string.Equals(tokenType, peekToken.Type);
    }

    private Precedence PeekPrecedence()
    {
        if (precidenceMap.TryGetValue(peekToken.Type, out Precedence precedence))
            return precedence;
        return Precedence.LOWEST;
    }

    private Precedence CurrentPrecedence()
    {
        if (precidenceMap.TryGetValue(currToken.Type, out Precedence currentPrecedence))
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
        
        errors.Add($"expected next token to be {tokenType}, got {peekToken.Type} instead");
        return false;
    }

    private void NoPrefixParseFunctionError(string tokenType)
    {
        errors.Add($"no prefix parse function for {tokenType} found");
    }

    private IExpressionNode? ParseArrayLiteral()
    {
        return new ArrayLiteral(currToken, ParseExpressionList(TokenConstants.Rbracket));
    }

    private IExpressionNode? ParseIndexExpression(IExpressionNode left) 
    {
        NextToken();
        var token = currToken;
        var indexExpression = ParseExpression(Precedence.LOWEST);
        return PeekExpected(TokenConstants.Rbracket) ? 
            new IndexExpression(token, left, indexExpression) : null;
    }

    private IExpressionNode? ParseExpression(Precedence precidence)
    {
        var foundParseFunction = prefixFunctions.TryGetValue(currToken.Type, 
            out PrefixParseFunction? prefixParseFunction);
        
        if (!foundParseFunction) 
        {
            NoPrefixParseFunctionError(currToken.Type);
            return null;
        }
        var leftExpression = prefixParseFunction();

        while (!PeekTokenIs(TokenConstants.Semicolon) && precidence < PeekPrecedence())
        {
            var infixFunction = infixFunctions[peekToken.Type];
            if (infixFunction == null)
                return leftExpression;

            NextToken();

            leftExpression = infixFunction(leftExpression);
        }

        return leftExpression;
    }

    private IExpressionNode? ParseIdentifier()
    {
        return new Identifier(currToken, currToken.Literal);
    }

    private IExpressionNode? ParseStringLiteral()
    {
        return new StringLiteral(currToken, currToken.Literal);
    }

    private IExpressionNode? ParseIntegerLiteral()
    {
        if (!long.TryParse(currToken.Literal, out long value))
        {
            var errorMessage = $"could not parse {currToken.Literal} as integer";
            errors.Add(errorMessage);
            return null;
        }

        return new IntegerLiteral(currToken, value);
    }

    private IExpressionNode? ParsePrefixExpression()
    {
        var token = currToken; 
        var tokenLiteral = currToken.Literal;

        NextToken();

        var right = ParseExpression(Precedence.PREFIX);

        return new PrefixExpression(token, tokenLiteral, right);
    }

    private IExpressionNode? ParseBoolean()
    {
        return new AST.Boolean(currToken, CurrentTokenIs(TokenConstants.True));
    }

    private IExpressionNode? ParseGroupExpression()
    {
        NextToken();

        var expression = ParseExpression(Precedence.LOWEST);
        if (!PeekExpected(TokenConstants.Rparen))
            return null;

        return expression;
    }

    private LetStatement? ParseLetStatement()
    {
        JoeToken token = currToken;
        Identifier name;
        IExpressionNode? value;

        if (!PeekExpected(TokenConstants.Ident))
            return null;

        name = new Identifier(currToken, currToken.Literal);

        if (!PeekExpected(TokenConstants.Assign)) 
            return null;

        NextToken();

        value = ParseExpression(Precedence.LOWEST);

        if (PeekTokenIs(TokenConstants.Semicolon))
            NextToken();

        return new LetStatement(token, name, value);
    }

    private ReturnStatement? ParseReturnStatement()
    {
        JoeToken token = currToken;
        IExpressionNode? returnValue;

        NextToken();

        returnValue = ParseExpression(Precedence.LOWEST);

        if (PeekTokenIs(TokenConstants.Semicolon))
            NextToken();

        return new ReturnStatement(token, returnValue);
    }

    private ExpressionStatement? ParseExpressionStatement()
    {
        JoeToken token = currToken;
        IExpressionNode? expression = ParseExpression(Precedence.LOWEST);

        if (PeekTokenIs(TokenConstants.Semicolon))
            NextToken();

        return new ExpressionStatement(token, expression);
    }

    private IStatementNode? ParseStatement()
    {
        switch (currToken.Type) 
        {
            case TokenConstants.Let:
                return ParseLetStatement();
            case TokenConstants.Return:
                return ParseReturnStatement();
            default:
                return ParseExpressionStatement();
        }
    }

    private BlockStatement ParseBlockStatement()
    {
        JoeToken token = currToken;
        List<IStatementNode> statements = new();

        NextToken();

        while (!CurrentTokenIs(TokenConstants.Rbrace) && !CurrentTokenIs(TokenConstants.Eof)) 
        {
            var statement = ParseStatement();
            if (statement != null)
                statements.Add(statement);
            NextToken();
        }

        return new BlockStatement(token, statements.ToArray());
    }

    private IExpressionNode? ParseIfExpression()
    {
        JoeToken token = currToken;

        IExpressionNode condition;
        BlockStatement consequence;
        BlockStatement? alternative = null;

        if (!PeekExpected(TokenConstants.Lparen))
            return null;

        NextToken();
        condition = ParseExpression(Precedence.LOWEST);

        if (!PeekExpected(TokenConstants.Rparen))
            return null;

        if (!PeekExpected(TokenConstants.Lbrace))
            return null;

        consequence = ParseBlockStatement();

        if (PeekTokenIs(TokenConstants.Else))
        {
            NextToken();

            if (!PeekExpected(TokenConstants.Lbrace))
                return null;

            alternative = ParseBlockStatement();
        }

        return new IfExpression(token, condition, consequence, alternative);
    }

    private Identifier[]? ParseFunctionParameters()
    {
        List<Identifier> identifiers = new List<Identifier>();

        if (PeekTokenIs(TokenConstants.Rparen))
        {
            NextToken();
            return identifiers.ToArray();
        }

        NextToken();

        var identifier = new Identifier(currToken, currToken.Literal);
        identifiers.Add(identifier);

        while (PeekTokenIs(TokenConstants.Comma))
        {
            NextToken();
            NextToken();
            identifier = new Identifier(currToken, currToken.Literal);
            identifiers.Add(identifier);
        }

        if (!PeekExpected(TokenConstants.Rparen))
            return null;

        return identifiers.ToArray();
    }

    private IExpressionNode? ParseFunctionLiteral()
    {
        JoeToken token = currToken;
        Identifier[]? functionParamaters;
        BlockStatement body;

        if (!PeekExpected(TokenConstants.Lparen))
            return null;

        functionParamaters = ParseFunctionParameters();

        if (!PeekExpected(TokenConstants.Lbrace))
            return null;

        body = ParseBlockStatement();

        return new FunctionLiteral(token, functionParamaters, body);
    }

    private InfixExpression ParseInfixExpression(IExpressionNode left)
    {
        JoeToken token = currToken;
        string op = currToken.Literal;
        IExpressionNode? right;

        var precedence = CurrentPrecedence();
        NextToken();
        right = ParseExpression(precedence);

        return new InfixExpression(token, left, op, right);
    }

    private IExpressionNode[]? ParseExpressionList(string endToken)
    {
        List<IExpressionNode> list = new();

        if (PeekTokenIs(endToken))
        {
            NextToken();
            return list.ToArray();
        }

        NextToken();

        list.Add(ParseExpression(Precedence.LOWEST));

        while (PeekTokenIs(TokenConstants.Comma)) 
        { 
            NextToken();
            NextToken();

            list.Add(ParseExpression(Precedence.LOWEST));
        }
        
        return PeekExpected(endToken) ? list.ToArray() : null;
    }

    private CallExpression ParseCallExpression(IExpressionNode function)
    {
        JoeToken token = currToken;
        IExpressionNode[]? arguments = ParseExpressionList(TokenConstants.Rparen);
        return new CallExpression(token, function, arguments);
    }
}
