using JoeLang.Constants;
using JoeLang.Evaluator;
using JoeLang.Lexer;
using JoeLang.Object;
using JoeLang.Parser;

namespace JoeLang.Tests;

public class EvaluatorUnitTests
{
    private struct IntegerTest
    {
        public string input;
        public long expected;
        public IntegerTest(string input, long expected) 
        { 
            this.input = input;
            this.expected = expected;
        }
    }

    private struct StringTest
    {
        public string input;
        public string expected;
        public StringTest(string input, string expected) 
        { 
            this.input = input;
            this.expected = expected;
        }
    }

    private struct DynamicTest
    {
        public string input;
        public dynamic? expected;
        public DynamicTest(string input, dynamic? expected)
        {
            this.input = input;
            this.expected = expected;
        }
    }

    private struct BooleanTest
    {
        public string input;
        public bool expected;
        public BooleanTest(string input, bool expected)
        {
            this.input = input;
            this.expected = expected;
        }
    }

    [Fact]
    public void TestClosures()
    {
        var input = 
            """
            var newAdder = fn(x) {
                fn(y) { x + y };
            };
            
            var addTwo = newAdder(2);
            addTwo(2);
            """;

        var evaluated = TestEvaluate(input);
        TestIntegerObject(evaluated, 4);
    }

    [Fact]
    public void TestFunctionObject()
    {
        var input = "fn(x) { x+2; };";

        var evaluated = TestEvaluate(input);
        Assert.IsType<JoeFunction>(evaluated);
        
        var fn = (JoeFunction)evaluated;
        Assert.Single(fn.Parameters);
        Assert.Equal("x", fn.Parameters[0].ToString());
        Assert.Equal("(x + 2)", fn.Body.ToString());
    }

    [Fact]
    public void TestFunctionApplication()
    {
        var tests = new IntegerTest[]
        {
            new("var identity = fn(x) { return x; }; identity(5);", 5),
            new("var identity = fn(x) { x; }; identity(5);", 5),
            new("var double = fn(x) { x * 2; }; double(5);", 10),
            new("var add = fn(x, y) { x + y; }; add(5, 5);", 10),
            new("var add = fn(x, y) { x + y; }; add(5 + 5, add(5, 5));", 20),
            new("fn(x) { x; }(5)", 5)
        };

        foreach (var test in tests)
        {
            var evaluated = TestEvaluate(test.input);
            TestIntegerObject(evaluated, test.expected);
        }
    }

    [Fact]
    public void TestIntegerExpressions()
    {
        var tests = new IntegerTest[]
        {
            new("5", 5),
            new("10", 10),
            new("-5", -5),
            new("-10", -10),
            new("5 + 5 + 5 + 5 - 10", 10),
            new("2 * 2 * 2 * 2 * 2", 32),
            new("-50 + 100 + -50", 0),
            new("5 * 2 + 10", 20),
            new("5 + 2 * 10", 25),
            new("20 + 2 * -10", 0),
            new("50 / 2 * 2 + 10", 60),
            new("2 * (5 + 10)", 30),
            new("3 * 3 * 3 + 10", 37),
            new("3 * (3 * 3) + 10", 37),
            new("(5 + 10 * 2 + 15 / 3) * 2 + -10", 50)
        };

        foreach (var test in tests) 
        {
            var evaluated = TestEvaluate(test.input);
            TestIntegerObject(evaluated, test.expected);
        }
    }

    [Fact]
    public void TestBooleanExpressions()
    {
        var tests = new BooleanTest[]
        {
            new("true", true),
            new("false", false),
            new("1 < 2", true),
            new("1 > 2", false),
            new("1 < 1", false),
            new("1 > 1", false),
            new("1 == 1", true),
            new("1 != 1", false),
            new("1 == 2", false),
            new("1 != 2", true),
            new("true == true", true),
            new("false == false", true),
            new("true == false", false),
            new("true != false", true),
            new("false != true", true),
            new("(1 < 2) == true", true),
            new("(1 < 2) == false", false),
            new("(1 > 2) == true", false),
            new("(1 > 2) == false", true)
        };

        foreach (var test in tests) 
        {
            var evaluated = TestEvaluate(test.input);
            TestBooleanObject(evaluated, test.expected);
        }
    }

    [Fact]
    public void TestIfElseExpressions()
    {
        var tests = new DynamicTest[]
        {
            new("if (true) { 10 }", (long)10),
            new("if (false) { 10 }", null),
            new("if (1) { 10 }", (long)10),
            new("if (1 < 2) { 10 }", (long)10),
            new("if (1 > 2) { 10 }", null),
            new("if (1 > 2) { 10 } else { 20 }", (long)20),
            new("if (1 < 2) { 10 } else { 20 }", (long)10)
        };

        foreach (var test in tests) 
        {
            var evaluated = TestEvaluate(test.input);

            if (test.expected is long)
                TestIntegerObject(evaluated, test.expected);
            else
                TestNullObject(evaluated);
        }
    }

    [Fact]
    public void TestBangOperator()
    {
        var tests = new BooleanTest[]
        {
            new("!true", false),
            new("!false", true),
            new("!5", false),
            new("!!true", true),
            new("!!false", false),
            new("!!5", true)
        };

        foreach (var test in tests) 
        { 
            var evaluated = TestEvaluate(test.input);
            TestBooleanObject(evaluated, test.expected);
        }
    }

    [Fact]
    public void TestReturnStatements()
    {
        var tests = new IntegerTest[]
        {
            new("return 10;", 10),
            new("return 10; 9;", 10),
            new("return 2 * 5; 9;", 10),
            new("9; return 3 * 5; 9;", 15),
            new("""
            if (10 > 1)
            {
                if (10 > 1)
                {
                    return 10;
                }
                return 1;
            }
            """,10)
        };

        foreach (var test in tests) 
        {
            var evaluated = TestEvaluate(test.input);
            TestIntegerObject(evaluated, test.expected);
        }
    }

    [Fact]
    public void TestErrorHandling()
    {
        var tests = new StringTest[]
        {
            new("5 + true;",
            "type mismatch: INTEGER + BOOLEAN"),
            new("5 + true; 5;",
			"type mismatch: INTEGER + BOOLEAN"),
		    new("-true",
			"unknown operator: -BOOLEAN"),
            new("true + false;",
			"unknown operator: BOOLEAN + BOOLEAN"),
            new("5; true + false; 5",
			"unknown operator: BOOLEAN + BOOLEAN"),
            new("if (10 > 1) { true + false; }",
			"unknown operator: BOOLEAN + BOOLEAN"),
            new(
            """
                if (10 > 1)
                {
                    if (10 > 1)
                    {
                        return true + false;
                    }
                    return 1;
                }
            """,
			"unknown operator: BOOLEAN + BOOLEAN"),
            new("foobar",
            "identifier not found: foobar")
        };

        foreach (var test in tests) 
        { 
            var evaluated = TestEvaluate(test.input);

            Assert.IsType<JoeError>(evaluated);
            Assert.Equal(test.expected, ((JoeError)evaluated).Message);
        }
    }

    [Fact]
    public void TestVarStatements()
    {
        var tests = new IntegerTest[]
        {
            new("var a = 5; a;", 5),
            new("var a = 5 * 5; a;", 25),
            new("var a = 5; var b = a; b;", 5),
            new("var a = 5; var b = a; var c = a + b + 5; c;", 15),
        };

        foreach (var test in tests) 
            TestIntegerObject(TestEvaluate(test.input), test.expected);
    }

    private IJoeObject? TestEvaluate(string input)
    {
        var lexer = new JoeLexer(input);
        var parser = new DescentParser(lexer);
        var program = parser.ParseProgram();
        var environment = new JoeEnvironment();
        var evaluator = new JoeEvaluator();

        return evaluator.Evaluate(program, environment);
    }

    private void TestNullObject(IJoeObject? input) 
    {
        Assert.IsType<JoeNull>(input);
    }

    private void TestIntegerObject(IJoeObject? obj,  long expected) 
    {
        Assert.IsType<JoeInteger>(obj);
        Assert.Equal(expected, ((JoeInteger)obj).Value);
    }

    private void TestBooleanObject(IJoeObject? obj, bool expected)
    {
        Assert.IsType<JoeBoolean>(obj);
        Assert.Equal(expected, ((JoeBoolean)obj).Value);
    }
}
