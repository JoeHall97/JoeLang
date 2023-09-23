﻿using JoeLang.AST;
using JoeLang.Lexer;
using JoeLang.Parser;
using System.Text;

namespace JoeLang.Tests;

public class DescentParserUnitTests
{
    private struct VarTest
    {
        public string input;
        public string expectedIdentifier;
        public dynamic expectedValue;

        public VarTest(string input, string expectedIdentifier, dynamic expectedValue)
        {
            this.input = input;
            this.expectedIdentifier = expectedIdentifier;
            this.expectedValue = expectedValue;
        }
    }

    private struct ReturnTest
    {
        public string input;
        public dynamic expectedValue;

        public ReturnTest(string input, dynamic expectedValue)
        {
            this.input = input;
            this.expectedValue = expectedValue;
        }
    }

    private struct PrefixTest
    {
        public string input;
        public string op;
        public dynamic value;

        public PrefixTest(string input, string op, dynamic value)
        {
            this.input = input;
            this.op = op;
            this.value = value;
        }
    }

    private struct OperatorPrecedenceTest
    {
        public string input;
        public string expected;

        public OperatorPrecedenceTest(string input, string expected)
        {
            this.input = input;
            this.expected = expected;
        }
    }

    private struct InfixTest
    {
        public string input;
        public string op;
        public dynamic left;
        public dynamic right;

        public InfixTest(string input, string op, dynamic left, dynamic right)
        {
            this.input = input;
            this.op = op;
            this.left = left;
            this.right = right;
        }
    }

    private struct BooleanTest
    {
        public string input;
        public bool expectedValue;

        public BooleanTest(string input, bool value)
        {
            this.input = input;
            this.expectedValue = value;
        }
    }

    private struct FunctionParameterTest
    {
        public string input;
        public string[] expecteParameters;

        public FunctionParameterTest(string input, string[] expecteParameters)
        {
            this.input = input;
            this.expecteParameters = expecteParameters;
        }
    }

    [Fact]
    public void TestReturnStatement()
    {
        var tests = new ReturnTest[]
        {
            new ReturnTest("return 5;", 5),
            new ReturnTest("return true;", true),
            new ReturnTest("return foobar;", "foobar")
        };

        foreach (var test in tests)
        {
            var lexer = new JoeLexer(test.input);
            var parser = new DescentParser(lexer);
            var program = parser.ParseProgram();

            var parserErrors = CheckParserErrors(parser);
            if (parserErrors != null)
                Assert.Fail(parserErrors);

            Assert.Single(program.Statements);
            Assert.IsType<AST.ReturnStatement>(program.Statements[0]);

            var returnStatement = (AST.ReturnStatement)program.Statements[0];

            Assert.Equal("return", returnStatement.TokenLiteral());

            TestLiteralExpression(returnStatement.ReturnValue, test.expectedValue);
        }
    }

    [Fact]
    public void TestVarStatements()
    {
        var tests = new VarTest[]
        {
            new VarTest("var x = 5;", "x", 5),
            new VarTest("var y = true;", "y", true),
            new VarTest("var foobar = y;", "foobar", "y"),
        };

        foreach (var test in tests)
        {
            var lexer = new JoeLexer(test.input);
            var parser = new DescentParser(lexer);
            var program = parser.ParseProgram();

            var parserErrors = CheckParserErrors(parser);
            if (parserErrors != null)
                Assert.Fail(parserErrors);

            Assert.Single(program.Statements);

            var statement = program.Statements[0];

            TestVarStatement(statement, test.expectedIdentifier);

            var value = ((VarStatement)statement).Value;
            TestLiteralExpression(value, test.expectedValue);
        }
    }

    [Fact]
    public void TestIdentifierExpression()
    {
        var input = "foobar";

        var lexer = new JoeLexer(input);
        var parser = new DescentParser(lexer);
        var program = parser.ParseProgram();

        var parserErrors = CheckParserErrors(parser);
        if (parserErrors != null)
            Assert.Fail(parserErrors);

        Assert.Single(program.Statements);
        Assert.IsType<AST.ExpressionStatement>(program.Statements[0]);

        var expression = (AST.ExpressionStatement)program.Statements[0];

        Assert.IsType<AST.Identifier>(expression.Expression);

        var identifier = (AST.Identifier)expression.Expression;

        Assert.Equal("foobar", identifier.Value);
        Assert.Equal("foobar", identifier.TokenLiteral());
    }

    [Fact]
    public void TestIntegerLiteralExpressions()
    {
        var input = "5;";

        var lexer = new JoeLexer(input);
        var parser = new DescentParser(lexer);
        var program = parser.ParseProgram();

        var parserErrors = CheckParserErrors(parser);
        if (parserErrors != null)
            Assert.Fail(parserErrors);

        Assert.Single(program.Statements);
        Assert.IsType<AST.ExpressionStatement>(program.Statements[0]);

        var statement = (AST.ExpressionStatement)program.Statements[0];

        Assert.IsType<AST.IntegerLiteral>(statement.Expression);

        var integerLiteral = (AST.IntegerLiteral)statement.Expression;

        Assert.Equal(5, integerLiteral.Value);
        Assert.Equal("5", integerLiteral.TokenLiteral());
    }

    [Fact]
    public void TestParsingPrefixExpression()
    {
        var prefixTests = new PrefixTest[]
        {
            new PrefixTest("!5;", "!", 5),
            new PrefixTest("-15", "-", 15),
            new PrefixTest("!foobar;", "!", "foobar"),
            new PrefixTest("-foobar;", "-", "foobar"),
            new PrefixTest("!true", "!", true),
            new PrefixTest("!false", "!", false),
        };

        foreach (var test in prefixTests)
        {
            var lexer = new JoeLexer(test.input);
            var parser = new DescentParser(lexer);
            var program = parser.ParseProgram();

            var parserErrors = CheckParserErrors(parser);
            if (parserErrors != null)
                Assert.Fail(parserErrors);

            Assert.Single(program.Statements);
            Assert.IsType<AST.ExpressionStatement>(program.Statements[0]);

            var expressionStatement = (AST.ExpressionStatement)program.Statements[0];

            Assert.IsType<AST.PrefixExpression>(expressionStatement.Expression);

            var expression = (AST.PrefixExpression)expressionStatement.Expression;

            Assert.Equal(test.op, expression.Operator);

            TestLiteralExpression(expression.Right, test.value);
        }
    }

    [Fact]
    public void TestParsingInfixExpression()
    {
        var infixTests = new InfixTest[]
        {
            new InfixTest("5+5;", "+", 5, 5),
            new InfixTest("5-5;", "-", 5, 5),
            new InfixTest("5*5;", "*", 5, 5),
            new InfixTest("5/5;", "/", 5, 5),
            new InfixTest("5>5;", ">", 5, 5),
            new InfixTest("5<5;", "<", 5, 5),
            new InfixTest("5==5;", "==", 5, 5),
            new InfixTest("5!=5;", "!=", 5, 5),
            new InfixTest("true==true;", "==", true, true),
            new InfixTest("false!=true;", "!=", false, true),
            new InfixTest("false==false;", "==", false, false),
        };

        foreach (var test in infixTests)
        {
            var lexer = new JoeLexer(test.input);
            var parser = new DescentParser(lexer);
            var program = parser.ParseProgram();

            var parserErrors = CheckParserErrors(parser);
            if (parserErrors != null)
                Assert.Fail(parserErrors);

            Assert.Single(program.Statements);
            Assert.IsType<AST.ExpressionStatement>(program.Statements[0]);

            var expressionStatement = (AST.ExpressionStatement)program.Statements[0];

            TestInfixExpression(expressionStatement.Expression, test.op, test.left, test.right);
        }
    }

    [Fact]
    public void TestOperatorPrecedenceParsing()
    {
        var operatorPrecedenceTests = new OperatorPrecedenceTest[]
        {
            new OperatorPrecedenceTest("-a * b", "((-a) * b)"),
            new OperatorPrecedenceTest("!-a", "(!(-a))"),
            new OperatorPrecedenceTest("a + b + c", "((a + b) + c)"),
            new OperatorPrecedenceTest("a + b - c", "((a + b) - c)"),
            new OperatorPrecedenceTest("a * b * c", "((a * b) * c)"),
            new OperatorPrecedenceTest("a * b / c", "((a * b) / c)"),
            new OperatorPrecedenceTest("a + b / c", "(a + (b / c))"),
            new OperatorPrecedenceTest("a + b * c + d / e - f", "(((a + (b * c)) + (d / e)) - f)"),
            new OperatorPrecedenceTest("3 + 4; -5 * 5", "(3 + 4)((-5) * 5)"),
            new OperatorPrecedenceTest("5 > 4 == 3 < 4", "((5 > 4) == (3 < 4))"),
            new OperatorPrecedenceTest("5 < 4 != 3 > 4", "((5 < 4) != (3 > 4))"),
            new OperatorPrecedenceTest("3 + 4 * 5 == 3 * 1 + 4 * 5", "((3 + (4 * 5)) == ((3 * 1) + (4 * 5)))"),
            new OperatorPrecedenceTest("true", "true"),
            new OperatorPrecedenceTest("false", "false"),
            new OperatorPrecedenceTest("3 > 5 == false",  "((3 > 5) == false)"),
            new OperatorPrecedenceTest("3 < 5 == true", "((3 < 5) == true)"),
            new OperatorPrecedenceTest("1 + (2 + 3) + 4", "((1 + (2 + 3)) + 4)"),
            new OperatorPrecedenceTest("(5 + 5) * 2", "((5 + 5) * 2)"),
            new OperatorPrecedenceTest("2 / (5 + 5)", "(2 / (5 + 5))"),
            new OperatorPrecedenceTest("(5 + 5) * 2 * (5 + 5)", "(((5 + 5) * 2) * (5 + 5))"),
            new OperatorPrecedenceTest("-(5 + 5)", "(-(5 + 5))"),
            new OperatorPrecedenceTest("!(true == true)", "(!(true == true))"),
            new OperatorPrecedenceTest("a + add(b * c) + d", "((a + add((b * c))) + d)"),
            new OperatorPrecedenceTest("add(a, b, 1, 2 * 3, 4 + 5, add(6, 7 * 8))", "add(a, b, 1, (2 * 3), (4 + 5), add(6, (7 * 8)))"),
            new OperatorPrecedenceTest("add(a + b + c * d / f + g)", "add((((a + b) + ((c * d) / f)) + g))"),
        };

        foreach (var test in operatorPrecedenceTests)
        {
            var lexer = new JoeLexer(test.input);
            var parser = new DescentParser(lexer);
            var program = parser.ParseProgram();

            var parserErrors = CheckParserErrors(parser);
            if (parserErrors != null)
                Assert.Fail(parserErrors);

            try
            {
                Assert.Equal(test.expected, program.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }

    [Fact]
    public void TestBooleanExpression()
    {
        var tests = new BooleanTest[]
        {
            new BooleanTest("true", true),
            new BooleanTest("false", false),
        };

        foreach (var test in tests)
        {
            var lexer = new JoeLexer(test.input);
            var parser = new DescentParser(lexer);
            var program = parser.ParseProgram();

            var parserErrors = CheckParserErrors(parser);
            if (parserErrors != null)
                Assert.Fail(parserErrors);

            Assert.Single(program.Statements);
            Assert.IsType<AST.ExpressionStatement>(program.Statements[0]);

            var expressionStatement = (AST.ExpressionStatement)program.Statements[0];

            Assert.IsType<AST.Boolean>(expressionStatement.Expression);

            var boolean = (AST.Boolean)expressionStatement.Expression;

            Assert.Equal(test.expectedValue, boolean.Value);
        }
    }

    [Fact]
    public void TestIfExpression()
    {
        var input = "if (x < y) { x }";
        var lexer = new JoeLexer(input);
        var parser = new DescentParser(lexer);
        var program = parser.ParseProgram();

        var parserErrors = CheckParserErrors(parser);
        if (parserErrors != null)
            Assert.Fail(parserErrors);

        Assert.Single(program.Statements);
        Assert.IsType<AST.ExpressionStatement>(program.Statements[0]);

        var expressionStatement = (AST.ExpressionStatement)program.Statements[0];

        Assert.IsType<AST.IfExpression>(expressionStatement.Expression);

        var ifExpression = (AST.IfExpression)expressionStatement.Expression;

        TestInfixExpression(ifExpression.Condition, "<", "x", "y");

        Assert.Single(ifExpression.Consequence.Statements);
        Assert.IsType<AST.ExpressionStatement>(ifExpression.Consequence.Statements[0]);

        expressionStatement = (AST.ExpressionStatement)ifExpression.Consequence.Statements[0];

        TestIdentifier(expressionStatement.Expression, "x");
        Assert.Null(ifExpression.Alternative);
    }

    [Fact]
    public void TestIfElseStatement()
    {
        var input = "if (x < y) { x } else { y }";
        var lexer = new JoeLexer(input);
        var parser = new DescentParser(lexer);
        var program = parser.ParseProgram();

        var parserErrors = CheckParserErrors(parser);
        if (parserErrors != null)
            Assert.Fail(parserErrors);

        Assert.Single(program.Statements);
        Assert.IsType<AST.ExpressionStatement>(program.Statements[0]);

        var expressionStatement = (AST.ExpressionStatement)program.Statements[0];

        Assert.IsType<AST.IfExpression>(expressionStatement.Expression);

        var ifExpression = (AST.IfExpression)expressionStatement.Expression;

        TestInfixExpression(ifExpression.Condition, "<", "x", "y");

        Assert.Single(ifExpression.Consequence.Statements);
        Assert.IsType<AST.ExpressionStatement>(ifExpression.Consequence.Statements[0]);

        var consequence = (AST.ExpressionStatement)ifExpression.Consequence.Statements[0];

        TestIdentifier(consequence.Expression, "x");

        Assert.NotNull(ifExpression.Alternative);
        Assert.Single(ifExpression.Alternative.Statements);
        Assert.IsType<AST.ExpressionStatement>(ifExpression.Alternative.Statements[0]);

        var alternative = (AST.ExpressionStatement)ifExpression.Alternative.Statements[0];

        TestIdentifier(alternative.Expression, "y");
    }

    [Fact]
    public void TestFunctionLiteral()
    {
        var input = "fn(x, y) { x + y; }";
        var lexer = new JoeLexer(input);
        var parser = new DescentParser(lexer);
        var program = parser.ParseProgram();

        var parserErrors = CheckParserErrors(parser);
        if (parserErrors != null)
            Assert.Fail(parserErrors);

        Assert.Single(program.Statements);
        Assert.IsType<AST.ExpressionStatement>(program.Statements[0]);
        Assert.IsType<AST.FunctionLiteral>(((AST.ExpressionStatement)program.Statements[0]).Expression);

        var functionLiteral = (AST.FunctionLiteral)((AST.ExpressionStatement)program.Statements[0]).Expression;

        Assert.Equal(2, functionLiteral.Parameters.Length);

        TestLiteralExpression(functionLiteral.Parameters[0], "x");
        TestLiteralExpression(functionLiteral.Parameters[1], "y");

        Assert.Single(functionLiteral.Body.Statements);
        Assert.IsType<AST.ExpressionStatement>(functionLiteral.Body.Statements[0]);

        var bodyStatement = (AST.ExpressionStatement)functionLiteral.Body.Statements[0];

        TestInfixExpression(bodyStatement.Expression, "+", "x", "y");
    }

    [Fact]
    public void TestFunctionParameter()
    {
        var tests = new FunctionParameterTest[]
        {
            new FunctionParameterTest("fn() {};", new string[]{ }),
            new FunctionParameterTest("fn(x) {};", new string[]{ "x" }),
            new FunctionParameterTest("fn(x, y, z) {};", new string[]{ "x", "y", "z" }),
        };

        foreach (var test in tests)
        {
            var lexer = new JoeLexer(test.input);
            var parser = new DescentParser(lexer);
            var program = parser.ParseProgram();

            var parserErrors = CheckParserErrors(parser);
            if (parserErrors != null)
                Assert.Fail(parserErrors);

            Assert.Single(program.Statements);
            Assert.IsType<AST.ExpressionStatement>(program.Statements[0]);
            Assert.IsType<AST.FunctionLiteral>(((AST.ExpressionStatement)program.Statements[0]).Expression);

            var functionLiteral = (AST.FunctionLiteral)((AST.ExpressionStatement)program.Statements[0]).Expression;

            Assert.Equal(test.expecteParameters.Length, functionLiteral.Parameters.Length);

            for (var i = 0; i < test.expecteParameters.Length; i++)
                TestLiteralExpression(functionLiteral.Parameters[i], test.expecteParameters[i]);
        }
    }

    [Fact]
    public void TestCallExpression()
    {
        var input = "add(1, 2 * 3, 4 + 5);";
        var lexer = new JoeLexer(input);
        var parser = new DescentParser(lexer);
        var program = parser.ParseProgram();

        var parserErrors = CheckParserErrors(parser);
        if (parserErrors != null) 
            Assert.Fail(parserErrors);

        Assert.Single(program.Statements);
        Assert.IsType<AST.ExpressionStatement>(program.Statements[0]);
        Assert.IsType<AST.CallExpression>(((AST.ExpressionStatement)program.Statements[0]).Expression);

        var callExpression = (AST.CallExpression)((AST.ExpressionStatement)program.Statements[0]).Expression;

        TestIdentifier(callExpression.Function, "add");

        Assert.Equal(3, callExpression.Arguments.Length);

        TestLiteralExpression(callExpression.Arguments[0], 1);
        TestInfixExpression(callExpression.Arguments[1], "*", "2", "3");
        TestInfixExpression(callExpression.Arguments[2], "+", "4", "5");
    }

    private static void TestInfixExpression(IExpressionNode expression, string op, dynamic left, dynamic right)
    {
        Assert.IsType<AST.InfixExpression>(expression);

        var infixExpression = (AST.InfixExpression)expression;

        TestLiteralExpression(infixExpression.Left, left);

        Assert.Equal(op, infixExpression.Operator);

        TestLiteralExpression(infixExpression.Right, right);
    }

    private static void TestLiteralExpression(IExpressionNode expression, dynamic expectedValue)
    {
        switch (expectedValue)
        {
            case string:
                TestIdentifier(expression, (string)expectedValue);
                break;
            case bool:
                TestBooleanLiteral(expression, (bool)expectedValue);
                break;
            case int:
            case long:
                // all integer values in the language are currently a long
                TestIntegerLiteral(expression, (long)expectedValue);
                break;
            default:
                Assert.Fail($"type of expression not handled. got={expectedValue.GetType().Name}");
                break;
        }
    }

    private static void TestIdentifier(IExpressionNode expression, string expectedValue)
    {
        Assert.IsType<AST.Identifier>(expression);

        AST.Identifier identifier = (AST.Identifier)expression;

        Assert.Equal(expectedValue, identifier.Value);
        Assert.Equal(expectedValue, identifier.TokenLiteral());
    }

    private static void TestIntegerLiteral(IExpressionNode expression, long expectedValue)
    {
        Assert.IsType<AST.IntegerLiteral>(expression);

        AST.IntegerLiteral integerLiteral = (AST.IntegerLiteral)expression;

        Assert.Equal(expectedValue, integerLiteral.Value);
        Assert.Equal(expectedValue.ToString(), integerLiteral.TokenLiteral());
    }

    private static void TestBooleanLiteral(IExpressionNode expression, bool expectedValue) 
    {
        Assert.IsType<AST.Boolean>(expression);

        AST.Boolean boolean = (AST.Boolean)expression;

        Assert.Equal(expectedValue, boolean.Value);
        Assert.Equal(expectedValue.ToString().ToLower(), boolean.TokenLiteral());
    }

    private static void TestVarStatement(IStatementNode statement, string expectedIdentifier)
    {
        Assert.Equal("var", statement.TokenLiteral());
        Assert.IsType<AST.VarStatement>(statement);

        VarStatement varStatement = (VarStatement)statement;

        Assert.Equal(expectedIdentifier, varStatement.Name.Value);
        Assert.Equal(expectedIdentifier, varStatement.Name.TokenLiteral());
    }

    private static string? CheckParserErrors(DescentParser p)
    {
        var errors = p.Errors;

        if (errors.Length == 0)
            return null;

        StringBuilder sb = new();

        sb.Append($"Parse had {errors.Length} errors\n");
        foreach (var error in errors)
            sb.Append($"   Parser had error: {error}\n");

        return sb.ToString();
    }
}
