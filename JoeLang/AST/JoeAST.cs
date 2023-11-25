using JoeLang.Token;
using System.Text;

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
    private readonly IStatementNode[] statements;

    public JoeProgram(IStatementNode[] statmenets)
    {
        this.statements = statmenets;
    }

    public string TokenLiteral() 
    { 
        if (statements.Length > 0)
            return statements[0].TokenLiteral();
        return ""; 
    }

    public override string ToString() 
    { 
        StringBuilder sb = new();

        foreach (var stat in statements)
            sb.Append(stat.ToString());

        return sb.ToString();
    }

    public IStatementNode[] Statements
    { 
        get => statements;
    }
}

public class Identifier : IExpressionNode
{
    private readonly JoeToken token;
    private readonly string value;

    public Identifier(JoeToken token, string value)
    {
        this.token = token;
        this.value = value;
    }

    public string Value
    {
        get => value;    
    }

    public string TokenLiteral() { return token.Literal; }

    public override string ToString() { return value; }
}

public class CallExpression : IExpressionNode
{
    private readonly JoeToken token;
    private readonly IExpressionNode function;
    private readonly IExpressionNode[] arguments;

    public CallExpression(JoeToken token, IExpressionNode function, IExpressionNode[] arguments)
    {
        this.token = token;
        this.function = function;
        this.arguments = arguments;
    }

    public IExpressionNode Function
    { 
        get => function;
    }

    public IExpressionNode[] Arguments
    { 
        get => arguments;    
    }

    public string TokenLiteral() { return token.Literal; }

    public override string ToString()
    {
        StringBuilder builder = new();
        var args = new string[arguments.Length];

        for (int i=0; i<arguments.Length; i++)
            args[i] = arguments[i].ToString();

        builder.Append(function.ToString());
        builder.Append('(');
        builder.Append(string.Join(", ", args));
        builder.Append(')');

        return builder.ToString();
    }
}

public class BlockStatement : IStatementNode
{
    private readonly JoeToken token;
    private readonly IStatementNode[] statements;

    public BlockStatement(JoeToken token, IStatementNode[] statements)
    {
        this.token = token;
        this.statements = statements;
    }

    public IStatementNode[] Statements
    { 
        get => statements; 
    }

    public string TokenLiteral() { return token.Literal; }

    public override string ToString()
    {
        StringBuilder builder = new();

        foreach (var statement in statements)
            builder.Append(statement.ToString());

        return builder.ToString();
    }
}

public class FunctionLiteral : IExpressionNode
{
    private readonly JoeToken token;
    private readonly Identifier[] parameters;
    private readonly BlockStatement body;

    public FunctionLiteral(JoeToken token, Identifier[] parameters, BlockStatement body)
    {
        this.token = token;
        this.parameters = parameters;
        this.body = body;
    }

    public Identifier[] Parameters
    {
        get => parameters;
    }

    public BlockStatement Body
    { 
        get => body; 
    }

    public string TokenLiteral() { return token.Literal; }

    public override string ToString() 
    {
        StringBuilder builder = new();

        var parameters = new string[this.parameters.Length];
        for (int i=0; i<parameters.Length; i++)
            parameters[i] = this.parameters[i].ToString();

        builder.Append(TokenLiteral());
        builder.Append('(');
        builder.Append(string.Join(", ", parameters));
        builder.Append(')');
        builder.Append(body.ToString());
        
        return builder.ToString();
    }
}

public class IfExpression : IExpressionNode
{
    private readonly JoeToken token;
    private readonly IExpressionNode condition;
    private readonly BlockStatement consequence;
    private readonly BlockStatement? alternative;

    public IfExpression(JoeToken token, IExpressionNode condition, BlockStatement consequence, BlockStatement? alternative)
    {
        this.token = token;
        this.condition = condition;
        this.consequence = consequence;
        this.alternative = alternative;
    }

    public IExpressionNode Condition
    { 
        get => condition; 
    }

    public BlockStatement Consequence
    { 
        get => consequence; 
    }

    public BlockStatement? Alternative
    { 
        get => alternative; 
    }

    public string TokenLiteral() { return token.Literal; }

    public override string ToString()
    {
        StringBuilder builder = new();

        builder.Append("if");
        builder.Append(condition.ToString());
        builder.Append(' ');
        builder.Append(consequence.ToString());

        if (alternative != null)
        {
            builder.Append("else");
            builder.Append(alternative.ToString());
        }

        return builder.ToString();
    }
}

public class Boolean : IExpressionNode
{
    private readonly JoeToken token;
    private readonly bool value;

    public Boolean(JoeToken token, bool value) 
    {  
        this.token = token; 
        this.value = value;
    }

    public bool Value
    {
        get => value;
    }

    public string TokenLiteral() { return token.Literal; }

    public override string ToString() { return token.Literal; }
}

public class VarStatement : IStatementNode
{
    private readonly JoeToken token;
    private readonly Identifier name;
    private readonly IExpressionNode value;

    public VarStatement(JoeToken token, Identifier name, IExpressionNode value)
    {
        this.token = token;
        this.name = name;
        this.value = value;
    }

    public Identifier Name
    {
        get => name;
    }

    public IExpressionNode Value
    {
        get => value;
    }

    public string TokenLiteral() { return token.Literal; }

    public override string ToString()
    {
        StringBuilder builder = new();

        builder.Append(TokenLiteral() + " ");
        builder.Append(name.ToString());
        builder.Append(" = ");

        if (value != null)
            builder.Append(value.ToString());

        builder.Append(';');

        return builder.ToString();
    }
}

public class ReturnStatement : IStatementNode
{
    private readonly JoeToken token;
    private readonly IExpressionNode returnValue;

    public ReturnStatement(JoeToken token, IExpressionNode returnValue)
    {
        this.token= token;
        this.returnValue = returnValue;
    }

    public IExpressionNode ReturnValue
    {
        get => returnValue;
    }

    public string TokenLiteral() { return token.Literal; }

    public override string ToString()
    {
        StringBuilder builder = new();

        builder.Append(TokenLiteral() + " ");

        if (returnValue != null)
            builder.Append(returnValue.ToString());

        builder.Append(';');

        return builder.ToString();
    }
}

public class ExpressionStatement : IStatementNode
{
    private readonly JoeToken token;
    private readonly IExpressionNode expression;

    public ExpressionStatement(JoeToken token, IExpressionNode expression)
    {
        this.token= token;
        this.expression= expression;
    }

    public IExpressionNode Expression
    { 
        get => expression;
    }

    public string TokenLiteral() { return token.Literal; }

    public override string ToString() 
    {
        return expression != null ? expression.ToString() : "";
    }
}

public class IntegerLiteral : IExpressionNode
{
    private readonly JoeToken token;
    private readonly long value;

    public IntegerLiteral(JoeToken token, long value) 
    {
        this.token = token;
        this.value= value;
    }

    public long Value
    {
        get => value;
    }

    public string TokenLiteral() { return token.Literal; }

    public override string ToString() { return token.Literal; }
}

public class StringLiteral : IExpressionNode
{
    private readonly JoeToken token;
    private readonly string value;

    public StringLiteral(JoeToken token, string value)
    {
        this.token = token;
        this.value = value;
    }

    public JoeToken Token { get => token; }
    public string Value { get => value; }

    public string TokenLiteral() { return token.Literal; }
    public override string ToString() { return token.Literal; }
}

public class PrefixExpression : IExpressionNode
{
    private readonly JoeToken token;
    private readonly string op;
    private readonly IExpressionNode right;

    public PrefixExpression(JoeToken token, string op, IExpressionNode right) 
    {
        this.token = token;
        this.op = op; 
        this.right= right;
    }

    public string Operator
    {
        get => op;
    }

    public IExpressionNode Right
    { 
        get => right; 
    }

    public string TokenLiteral() { return token.Literal; }

    public override string ToString()
    {
        StringBuilder builder = new();

        builder.Append('(');
        builder.Append(op);
        builder.Append(right.ToString());
        builder.Append(')');

        return builder.ToString();
    }
}

public class InfixExpression : IExpressionNode
{
    private readonly JoeToken token;
    private readonly IExpressionNode left;
    private readonly string op;
    private readonly IExpressionNode right;

    public InfixExpression(JoeToken token, IExpressionNode left, string op, IExpressionNode right)
    {
        this.token = token;
        this.left = left;
        this.op = op;
        this.right = right;
    }

    public string Operator
    { 
        get => op; 
    }

    public IExpressionNode Left
    {
        get => left;
    }

    public IExpressionNode Right
    {
        get => right;
    }

    public string TokenLiteral() { return token.Literal; }

    public override string ToString()
    {
        StringBuilder builder = new();

        builder.Append('(');
        builder.Append(left.ToString());
        builder.Append($" {op} ");
        builder.Append(right.ToString());
        builder.Append(')');

        return builder.ToString();
    }
}