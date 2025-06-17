using JoeLang.Lexer;
using JoeLang.Constants;

namespace JoeLang.Tests;

public class LexerUnitTests
{
	private struct TokenTest(string type, string literal)
	{
		public readonly string ExpectedType = type;
		public readonly string ExpectedLiteral = literal;
	}

	[Fact]
    public void TestNextToken()
    {
        var input = """
                    let five = 5;
                    let ten = 10;
                    
                    // test to make sure that comments are skipped
                    let add = fn(x, y) {
                        x + y;
                    };
                    
                    let result = add(five, ten);
                    !-/*5;
                    5 < 10 > 5;
                    if (5 < 10) {
                        return true;
                    } else {
                        return false;
                    }
                    	
                    10 == 10;
                    10 != 9;
                    [1, 2];
                    """;
        input += "\"foo bar\"\n\"foobar\"";
        input += "{\"foo\": \"bar\"}";

        var tests = new TokenTest[]
        {
            // let five = 5;
		    new(TokenConstants.Let, "let"),
            new(TokenConstants.Ident, "five"),
            new(TokenConstants.Assign, "="),
            new(TokenConstants.Int, "5"),
            new(TokenConstants.Semicolon, ";"),
            // let ten = 10;
            new(TokenConstants.Let, "let"),
            new(TokenConstants.Ident, "ten"),
            new(TokenConstants.Assign, "=")   ,
            new(TokenConstants.Int, "10"),
            new(TokenConstants.Semicolon, ";"),
            // let add = fn(x, y) {
            // 		x + y;
            // };
            new(TokenConstants.Let, "let"),
            new(TokenConstants.Ident, "add"),
            new(TokenConstants.Assign, "="),
            new(TokenConstants.Function, "fn"),
            new(TokenConstants.Lparen, "("),
            new(TokenConstants.Ident, "x"),
            new(TokenConstants.Comma, ","),
            new(TokenConstants.Ident, "y") ,
            new(TokenConstants.Rparen, ")"),
            new(TokenConstants.Lbrace, "{"),
            new(TokenConstants.Ident, "x"),
            new(TokenConstants.Plus, "+"),
            new(TokenConstants.Ident, "y"),
            new(TokenConstants.Semicolon, ";"),
            new(TokenConstants.Rbrace, "}"),
            new(TokenConstants.Semicolon, ";"),
            // let result = add(five, ten);
		    new(TokenConstants.Let, "let"),
            new(TokenConstants.Ident, "result"),
            new(TokenConstants.Assign, "="),
            new(TokenConstants.Ident, "add"),
            new(TokenConstants.Lparen, "("),
            new(TokenConstants.Ident, "five"),
            new(TokenConstants.Comma, ","),
            new(TokenConstants.Ident, "ten"),
            new(TokenConstants.Rparen, ")"),
            new(TokenConstants.Semicolon, ";"),
            // !-/*5;
		    new(TokenConstants.Bang, "!"),
            new(TokenConstants.Minus, "-"),
            new(TokenConstants.Slash, "/"),
            new(TokenConstants.Asterisk, "*"),
            new(TokenConstants.Int, "5"),
            new(TokenConstants.Semicolon, ";"),
            // 5 < 10 > 5;
            new(TokenConstants.Int, "5"),
            new(TokenConstants.Lt, "<"),
            new(TokenConstants.Int, "10"),
            new(TokenConstants.Gt, ">"),
            new(TokenConstants.Int, "5"),
            new(TokenConstants.Semicolon, ";"),
            // if (5 < 10) {
            // 		return true;
            // } else {
            // 		return false;
            // }
            new(TokenConstants.If, "if"),
            new(TokenConstants.Lparen, "("),
            new(TokenConstants.Int, "5"),
            new(TokenConstants.Lt, "<"),
            new(TokenConstants.Int, "10"),
            new(TokenConstants.Rparen, ")"),
            new(TokenConstants.Lbrace, "{"),
            new(TokenConstants.Return, "return"),
            new(TokenConstants.True, "true"),
            new(TokenConstants.Semicolon, ";"),
            new(TokenConstants.Rbrace, "}"),
            new(TokenConstants.Else, "else"),
            new(TokenConstants.Lbrace, "{"),
            new(TokenConstants.Return, "return"),
            new(TokenConstants.False, "false"),
            new(TokenConstants.Semicolon, ";"),
            new(TokenConstants.Rbrace, "}"),
            // 10 == 10;
            new(TokenConstants.Int, "10"),
            new(TokenConstants.Eq, "=="),
            new(TokenConstants.Int, "10"),
            new(TokenConstants.Semicolon, ";"),
		    // 10 != 9;
		    new(TokenConstants.Int, "10"),
            new(TokenConstants.NotEq, "!="),
            new(TokenConstants.Int, "9"),
            new(TokenConstants.Semicolon, ";"),
		    // [1, 2];
		    new(TokenConstants.Lbracket, "["),
            new(TokenConstants.Int, "1"),
            new(TokenConstants.Comma, ","),
            new(TokenConstants.Int, "2"),
            new(TokenConstants.Rbracket, "]"),
            new(TokenConstants.Semicolon, ";"),
            // "foo bar"
            // "foobar"
            new(TokenConstants.String, "foo bar"),
            new(TokenConstants.String, "foobar"),
            // {\"foo\": \"bar\"}
		    new(TokenConstants.Lbrace, "{"),
            new(TokenConstants.String, "foo"),
            new(TokenConstants.Colon, ":"),
            new(TokenConstants.String, "bar"),
            new(TokenConstants.Rbrace, "}"),

            new(TokenConstants.Eof, ""),
        };

        var lexer = new JoeLexer(input);

        for (int i = 0; i < tests.Length; i++) 
        { 
            var token = lexer.NextToken();

            Assert.Equal(tests[i].ExpectedType, token.Type);
            Assert.Equal(tests[i].ExpectedLiteral, token.Literal);
        }
    }
}