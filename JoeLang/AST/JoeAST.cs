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

public class HashLiteral : IExpressionNode
{
    private readonly JoeToken token;
    private Dictionary<IExpressionNode, IExpressionNode> pairs;

    public HashLiteral(JoeToken token, Dictionary<IExpressionNode, IExpressionNode> pairs)
    {
        this.token = token;
        this.pairs = pairs;
    }

    public JoeToken Token { get => token; }
    public Dictionary<IExpressionNode, IExpressionNode> Pairs { get => pairs; }

    public string TokenLiteral() { return token.Literal; }
    public override string ToString() 
    { 
        string res = string.Join(", ", 
            pairs.Select(p => p.Key.ToString() + ":" + p.Value.ToString()));
        return res; 
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

    public override string ToString() { return $"{function.ToString()}({string.Join(", ", arguments.Select(a => a.ToString()))})"; }
}

public class ArrayLiteral : IExpressionNode
{ 
    private readonly JoeToken token;
    private readonly IExpressionNode[]? elements;

    public ArrayLiteral(JoeToken token, IExpressionNode[]? elements)
    {
        this.token = token;
        this.elements = elements;
    }

    public JoeToken Token { get => token; }
    public IExpressionNode[]? Elements { get => elements; }

    public string TokenLiteral() { return token.Literal; }
    public override string ToString() { return $"[{string.Join(", ", elements.Select(s => s.ToString()))}]"; }
}

public class IndexExpression : IExpressionNode
{
    private readonly JoeToken token;
    private readonly IExpressionNode left;
    private readonly IExpressionNode index;

    public IndexExpression(JoeToken token, IExpressionNode left, IExpressionNode index)
    {
        this.token = token;
        this.left = left;
        this.index = index;
    }

    public JoeToken Token { get => token; }
    public IExpressionNode Left { get => left; }
    public IExpressionNode Index { get => index; }

    public string TokenLiteral() { return token.Literal; }
    public override string ToString() { return $"({left.ToString()}[{index.ToString()}])"; }
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

    public override string ToString() { return string.Join("", statements.Select(s => s.ToString())); }
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
        return $"{TokenLiteral()}({string.Join(", ", parameters.Select(p => p.ToString()))}){body}";
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

    public override string ToString() { return alternative == null ? $"if{condition} {consequence}" : $"if{condition} {consequence}else{alternative}"; }
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

    public override string ToString() { return $"{TokenLiteral()} {name} = {value};"; }
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

    public override string ToString() { return $"{TokenLiteral()} {returnValue};"; }
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

    public override string ToString() { return expression != null ? expression.ToString() : ""; }
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

    public override string ToString() { return $"({op}{right})"; }
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

    public override string ToString() { return $"({left} {op} {right})"; }
}