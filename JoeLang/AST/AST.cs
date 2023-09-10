using JoeLang.Token;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JoeLang.AST;

public interface INode
{
    public string TokenLiteral();
    public string ToString();
}
public interface IStatementNode : INode { }
public interface IExpressionNode : INode { }

/* Need to do some thinking about whether to use structs or classes for the nodes in the AST.
 * Structs would be more performant due to the be stack allocated. Looking at the MS documentation 
 * (https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/choosing-between-class-and-struct)
 * it recommends using structs if:
 * 1) It logically represents a single value, similar to primitive types (int, double, etc.).
 * 2) It has an instance size under 16 bytes.
 * 3) It is immutable.
 * 4) It will not have to be boxed frequently.
 * The Nodes should statisfy 3 and 4, but 1 and 2 would not be met by many of them. Given this, I 
 * will implement classes now, but may come back in the future if I think structs would work better.
*/

public class JoeProgram : INode
{
    private readonly IStatementNode[] _statmenets;

    public JoeProgram(IStatementNode[] statmenets)
    {
        _statmenets = statmenets;
    }

    public string TokenLiteral() 
    { 
        if (_statmenets.Length > 0)
            return _statmenets[0].TokenLiteral();
        return ""; 
    }

    public override string ToString() 
    { 
        StringBuilder sb = new();

        foreach (var stat in _statmenets)
            sb.Append(stat.ToString());

        return sb.ToString();
    }
}

public class Identifier : IExpressionNode
{
    private readonly JoeToken _token;
    private readonly string _value;

    public Identifier(JoeToken token, string value)
    {
        _token = token;
        _value = value;
    }

    public string TokenLiteral() { return _token.Literal; }

    public override string ToString() { return _value; }
}

public class CallExpression : IExpressionNode
{
    private readonly JoeToken _token;
    private readonly IExpressionNode _function;
    private readonly IExpressionNode[] _arguments;

    public CallExpression(JoeToken token, IExpressionNode function, IExpressionNode[] arguments)
    {
        _token = token;
        _function = function;
        _arguments = arguments;
    }

    public string TokenLiteral() { return _token.Literal; }

    public override string ToString()
    {
        StringBuilder builder = new();
        var args = new string[_arguments.Length];

        for (int i=0; i<_arguments.Length; i++)
            args[i] = _arguments[i].ToString();

        builder.Append(_function.ToString());
        builder.Append("(");
        builder.Append(string.Join(", ", args));
        builder.Append(")");

        return builder.ToString();
    }
}

public class BlockStatement : IStatementNode
{
    private readonly JoeToken _token;
    private readonly IStatementNode[] _statements;

    public BlockStatement(JoeToken token, IStatementNode[] statements)
    {
        _token = token;
        _statements = statements;
    }

    public string TokenLiteral() { return _token.Literal; }

    public override string ToString()
    {
        StringBuilder builder = new();

        foreach (var statement in _statements)
            builder.Append(statement.ToString());

        return builder.ToString();
    }
}

public class FunctionLiteral : IExpressionNode
{
    private readonly JoeToken _token;
    private readonly Identifier[] _parameters;
    private readonly BlockStatement _body;

    public FunctionLiteral(JoeToken token, Identifier[] parameters, BlockStatement body)
    {
        _token = token;
        _parameters = parameters;
        _body = body;
    }

    public string TokenLiteral() { return _token.Literal; }

    public override string ToString() 
    {
        StringBuilder builder = new();

        var parameters = new string[_parameters.Length];
        for (int i=0; i<parameters.Length; i++)
            parameters[i] = _parameters[i].ToString();

        builder.Append(TokenLiteral());
        builder.Append('(');
        builder.Append(string.Join(", ", parameters));
        builder.Append(')');
        builder.Append(_body.ToString());
        
        return builder.ToString();
    }
}

public class IfExpression : IExpressionNode
{
    private readonly JoeToken _token;
    private readonly IExpressionNode _condition;
    private readonly BlockStatement _consequence;
    private readonly BlockStatement _alternative;

    public IfExpression(JoeToken token, IExpressionNode condition, BlockStatement consequence, BlockStatement alternative)
    {
        _token = token;
        _condition = condition;
        _consequence = consequence;
        _alternative = alternative;
    }

    public string TokenLiteral() { return _token.Literal; }

    public override string ToString()
    {
        StringBuilder builder = new();

        builder.Append("if");
        builder.Append(_condition.ToString());
        builder.Append(' ');
        builder.Append(_consequence.ToString());

        if (_alternative != null)
        {
            builder.Append("else");
            builder.Append(_alternative.ToString());
        }

        return builder.ToString();
    }
}

public class Boolean : IExpressionNode
{
    private readonly JoeToken _token;
    private readonly bool _value;

    public Boolean(JoeToken token, bool value) 
    {  
        _token = token; 
        _value = value;
    }

    public string TokenLiteral() { return _token.Literal; }

    public override string ToString() { return _token.Literal; }
}

public class LetStatement : IStatementNode
{
    private readonly JoeToken _token;
    private readonly Identifier _name;
    private readonly IExpressionNode _value;

    public LetStatement(JoeToken token, Identifier name, IExpressionNode value)
    {
        _token = token;
        _name = name;
        _value = value;
    }

    public string TokenLiteral() { return _token.Literal; }

    public override string ToString()
    {
        StringBuilder builder = new();

        builder.Append(TokenLiteral() + " ");
        builder.Append(_name.ToString());
        builder.Append(" = ");

        if (_value != null)
            builder.Append(_value.ToString());

        builder.Append(';');

        return builder.ToString();
    }
}

public class ReturnStatement : IStatementNode
{
    private readonly JoeToken _token;
    private readonly IExpressionNode _returnValue;

    public ReturnStatement(JoeToken token, IExpressionNode returnValue)
    {
        _token= token;
        _returnValue = returnValue;
    }

    public string TokenLiteral() { return _token.Literal; }

    public override string ToString()
    {
        StringBuilder builder = new();

        builder.Append(TokenLiteral() + " ");

        if (_returnValue != null)
            builder.Append(_returnValue.ToString());

        builder.Append(';');

        return builder.ToString();
    }
}

public class ExpressionStatement : IStatementNode
{
    private readonly JoeToken _token;
    private readonly IExpressionNode _expression;

    public ExpressionStatement(JoeToken token, IExpressionNode expression)
    {
        _token= token;
        _expression= expression;
    }

    public string TokenLiteral() { return _token.Literal; }

    public override string ToString() 
    {
        return _expression != null ? _expression.ToString() : "";
    }
}

public class IntegerLiteral : IExpressionNode
{
    private readonly JoeToken _token;
    private readonly Int64 _value;

    public IntegerLiteral(JoeToken token, Int64 value) 
    {
        _token = token;
        _value= value;
    }

    public string TokenLiteral() { return _token.Literal; }

    public override string ToString() { return _token.Literal; }
}

public class PrefixExpression : IExpressionNode
{
    private readonly JoeToken _token;
    private readonly string _operator;
    private readonly IExpressionNode _right;

    public PrefixExpression(JoeToken token, string op, IExpressionNode right) 
    {
        _token = token;
        _operator= op; 
        _right= right;
    }

    public string TokenLiteral() { return _token.Literal; }

    public override string ToString()
    {
        StringBuilder builder = new();

        builder.Append('(');
        builder.Append(_operator);
        builder.Append(_right.ToString());
        builder.Append(')');

        return builder.ToString();
    }
}

public class InfixExpression : IExpressionNode
{
    private readonly JoeToken _token;
    private readonly IExpressionNode _left;
    private readonly string _operator;
    private readonly IExpressionNode _right;

    public InfixExpression(JoeToken token, IExpressionNode left, string op, IExpressionNode right)
    {
        _token = token;
        _left = left;
        _operator = op;
        _right = right;
    }

    public string TokenLiteral() { return _token.Literal; }

    public override string ToString()
    {
        StringBuilder builder = new();

        builder.Append('(');
        builder.Append(_left.ToString());
        builder.Append(' ' + _operator + ' ');
        builder.Append(_right.ToString());
        builder.Append(')');

        return builder.ToString();
    }
}