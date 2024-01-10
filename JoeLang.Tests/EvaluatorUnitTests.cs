using JoeLang.Constants;
using JoeLang.Evaluator;
using JoeLang.Lexer;
using JoeLang.Object;
using JoeLang.Parser;
using System.IO.Pipes;

namespace JoeLang.Tests;

public class EvaluatorUnitTests
{
    private struct IntegerTest
    {
        public string input;
        public long? expected;
        public IntegerTest(string input, long? expected) 
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
    public void TestHashLiterals()
    {
        var input = """
         let two = "two";
         {
             "one": 10 - 9,
             two: 1 + 1,
             "thr" + "ee": 6 / 2,
             4: 4,
             true: 5,
             false: 6
         }
         """;

        var evaluated = TestEvaluate(input);
        Assert.IsType<JoeHash>(evaluated);
        var results = (JoeHash)evaluated;

        var expected = new Dictionary<HashKey, long>()
        {
            { (new JoeString("one")).HashKey(), 1 },
            { (new JoeString("two")).HashKey(), 2 },
            { (new JoeString("three")).HashKey(), 3 },
            { (new JoeInteger(4)).HashKey(), 4 },
            { EvaluatorConstants.TRUE.HashKey(), 5 },
            { EvaluatorConstants.FALSE.HashKey(), 6 },
        };

        Assert.Equal(expected.Count, results.Pairs.Count);

        foreach (var pair in expected) 
        {
            if (results.Pairs.TryGetValue(pair.Key, out HashPair hashPair))
                TestIntegerObject(hashPair.value, pair.Value);
            else
                Assert.Fail($"no pair for given key: {pair.Key}");
        }
    }

    [Fact]
    public void TestHashIndexExpressions()
    {
        var tests = new DynamicTest[]
        {
            new("{\"foo\": 5}[\"foo\"]", (long)5),
            new("{\"foo\": 5}[\"bar\"]", null),
            new("let key = \"foo\"; {\"foo\": 5}[key]", (long)5),
            new("{}[\"foo\"]", null),
            new("{5: 5}[5]", (long)5),
            new("{true: 5}[true]", (long)5),
        };

        foreach (var test in tests)
        {
            var evaluated = TestEvaluate(test.input);
            if (test.expected is long expectedLong)
                TestIntegerObject(evaluated, expectedLong);
            else
                TestNullObject(evaluated);
        }
    }

    [Fact]
    public void TestArrayLiterals()
    {
        var input = "[1, 2*2, 3+3]";
        var evaluated = TestEvaluate(input);

        Assert.IsType<JoeArray>(evaluated);
        
        var array = (JoeArray)evaluated;
        Assert.Equal(3, array.Elements.Length);

        TestIntegerObject(array.Elements[0], 1);
        TestIntegerObject(array.Elements[1], 4);
        TestIntegerObject(array.Elements[2], 6);
    }

    [Fact] public void TestArrayIndexExpressions() 
    {
        var tests = new IntegerTest[]
        {
            new("[1, 2, 3][0]", 1),
            new("[1, 2, 3][1]", 2),
            new("[1, 2, 3][2]", 3),
            new("let i = 0; [1][i];", 1),
            new("[1, 2, 3][1 + 1];", 3),
            new("let array = [1, 2, 3]; array[2];", 3),
            new("let array = [1, 2, 3]; array[0] + array[1] + array[2];", 6),
            new("let array = [1, 2, 3]; let i = array[0]; array[i]", 2),
            new("[1, 2, 3][3]", null),
            new("[1, 2, 3][-1]", null)
        };

        foreach (var test in tests)
        {
            var evaluated = TestEvaluate(test.input);
            if (test.expected != null)
                TestIntegerObject(evaluated, test.expected.GetValueOrDefault());
            else
                TestNullObject(evaluated);
        }
    }

    [Fact]
    public void TestClosures()
    {
        var input = 
            """
            let newAdder = fn(x) {
                fn(y) { x + y };
            };
            
            let addTwo = newAdder(2);
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
            new("let identity = fn(x) { return x; }; identity(5);", 5),
            new("let identity = fn(x) { x; }; identity(5);", 5),
            new("let double = fn(x) { x * 2; }; double(5);", 10),
            new("let add = fn(x, y) { x + y; }; add(5, 5);", 10),
            new("let add = fn(x, y) { x + y; }; add(5 + 5, add(5, 5));", 20),
            new("fn(x) { x; }(5)", 5)
        };

        foreach (var test in tests)
        {
            var evaluated = TestEvaluate(test.input);
            TestIntegerObject(evaluated, test.expected.GetValueOrDefault());
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
            TestIntegerObject(evaluated, test.expected.GetValueOrDefault());
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
    public void TestStringLiteral()
    {
        var input = "\"Hello World!\"";

        var evaluated = TestEvaluate(input);
        Assert.IsType<JoeString>(evaluated);
        Assert.Equal("Hello World!", ((JoeString)evaluated).Value);
    }

    [Fact]
    public void TestStringConcatenation()
    {
        var input = "\"Hello\" + \" \" + \"World!\"";

        var evaluated = TestEvaluate(input);
        Assert.IsType<JoeString>(evaluated);
        Assert.Equal("Hello World!", ((JoeString)evaluated).Value);
    }

    [Fact]
    public void TestBultinFunction()
    {
        var tests = new DynamicTest[]
        {
            new("len(\"\")", (long)0),
            new("len(\"four\")", (long)4),
            new("len(\"hello world\")", (long)11),
            new("len(1)", "argument to 'len' not supported. got=INTEGER"),
            new("len(\"one\", \"two\")", "wrong number of arguments. got=2, want=1")
        };

        foreach (var test in tests) 
        {
            var evaluated = TestEvaluate(test.input);

            switch (test.expected)
            {
                case long:
                    TestIntegerObject(evaluated, (long)test.expected);
                    break;
                case string:
                    Assert.IsType<JoeError>(evaluated);
                    Assert.Equal(test.expected, ((JoeError)evaluated).Message);
                    break;
            }
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
            TestIntegerObject(evaluated, test.expected.GetValueOrDefault());
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
            "identifier not found: foobar"),
            new("\"Hello\" - \"World\"", "unknown operator: STRING - STRING"),
            new("{\"name\": \"Monkey\"}[fn(x) { x }];", "unusable as hash key: FUNCTION")
        };

        foreach (var test in tests) 
        { 
            var evaluated = TestEvaluate(test.input);

            Assert.IsType<JoeError>(evaluated);
            Assert.Equal(test.expected, ((JoeError)evaluated).Message);
        }
    }

    [Fact]
    public void TestLetStatements()
    {
        var tests = new IntegerTest[]
        {
            new("let a = 5; a;", 5),
            new("let a = 5 * 5; a;", 25),
            new("let a = 5; let b = a; b;", 5),
            new("let a = 5; let b = a; let c = a + b + 5; c;", 15),
        };

        foreach (var test in tests) 
            TestIntegerObject(TestEvaluate(test.input), test.expected.GetValueOrDefault());
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
