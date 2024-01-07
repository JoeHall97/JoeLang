using JoeLang.Lexer;
using JoeLang.Constants;

namespace JoeLang.Tests;

public class LexerUnitTests
{
	private struct TokenTest
	{
        public TokenTest(string type, string literal)
        {
            expectedType = type;
            expectedLiteral = literal;
        }

		public string expectedType;
		public string expectedLiteral;
	}

	[Fact]
    public void TestNextToken()
    {
        var input = @"var five = 5;
        var ten = 10;

        // test to make sure that comments are skipped
        var add = fn(x, y) {
            x + y;
        };

        var result = add(five, ten);
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
        ";
        input += "\"foo bar\"\n\"foobar\"";
        input += "{\"foo\": \"bar\"}";

        var tests = new TokenTest[]
        {
            // var five = 5;
		    new TokenTest(TokenConstants.VAR, "var"),
            new TokenTest(TokenConstants.IDENT, "five"),
            new TokenTest(TokenConstants.ASSIGN, "="),
            new TokenTest(TokenConstants.INT, "5"),
            new TokenTest(TokenConstants.SEMICOLON, ";"),
            // var ten = 10;
            new TokenTest(TokenConstants.VAR, "var"),
            new TokenTest(TokenConstants.IDENT, "ten"),
            new TokenTest(TokenConstants.ASSIGN, "=")   ,
            new TokenTest(TokenConstants.INT, "10"),
            new TokenTest(TokenConstants.SEMICOLON, ";"),
            // var add = fn(x, y) {
            // 		x + y;
            // };
            new TokenTest(TokenConstants.VAR, "var"),
            new TokenTest(TokenConstants.IDENT, "add"),
            new TokenTest(TokenConstants.ASSIGN, "="),
            new TokenTest(TokenConstants.FUNCTION, "fn"),
            new TokenTest(TokenConstants.LPAREN, "("),
            new TokenTest(TokenConstants.IDENT, "x"),
            new TokenTest(TokenConstants.COMMA, ","),
            new TokenTest(TokenConstants.IDENT, "y") ,
            new TokenTest(TokenConstants.RPAREN, ")"),
            new TokenTest(TokenConstants.LBRACE, "{"),
            new TokenTest(TokenConstants.IDENT, "x"),
            new TokenTest(TokenConstants.PLUS, "+"),
            new TokenTest(TokenConstants.IDENT, "y"),
            new TokenTest(TokenConstants.SEMICOLON, ";"),
            new TokenTest(TokenConstants.RBRACE, "}"),
            new TokenTest(TokenConstants.SEMICOLON, ";"),
            // var result = add(five, ten);
		    new TokenTest(TokenConstants.VAR, "var"),
            new TokenTest(TokenConstants.IDENT, "result"),
            new TokenTest(TokenConstants.ASSIGN, "="),
            new TokenTest(TokenConstants.IDENT, "add"),
            new TokenTest(TokenConstants.LPAREN, "("),
            new TokenTest(TokenConstants.IDENT, "five"),
            new TokenTest(TokenConstants.COMMA, ","),
            new TokenTest(TokenConstants.IDENT, "ten"),
            new TokenTest(TokenConstants.RPAREN, ")"),
            new TokenTest(TokenConstants.SEMICOLON, ";"),
            // !-/*5;
		    new TokenTest(TokenConstants.BANG, "!"),
            new TokenTest(TokenConstants.MINUS, "-"),
            new TokenTest(TokenConstants.SLASH, "/"),
            new TokenTest(TokenConstants.ASTERISK, "*"),
            new TokenTest(TokenConstants.INT, "5"),
            new TokenTest(TokenConstants.SEMICOLON, ";"),
            // 5 < 10 > 5;
            new TokenTest(TokenConstants.INT, "5"),
            new TokenTest(TokenConstants.LT, "<"),
            new TokenTest(TokenConstants.INT, "10"),
            new TokenTest(TokenConstants.GT, ">"),
            new TokenTest(TokenConstants.INT, "5"),
            new TokenTest(TokenConstants.SEMICOLON, ";"),
            // if (5 < 10) {
            // 		return true;
            // } else {
            // 		return false;
            // }
            new TokenTest(TokenConstants.IF, "if"),
            new TokenTest(TokenConstants.LPAREN, "("),
            new TokenTest(TokenConstants.INT, "5"),
            new TokenTest(TokenConstants.LT, "<"),
            new TokenTest(TokenConstants.INT, "10"),
            new TokenTest(TokenConstants.RPAREN, ")"),
            new TokenTest(TokenConstants.LBRACE, "{"),
            new TokenTest(TokenConstants.RETURN, "return"),
            new TokenTest(TokenConstants.TRUE, "true"),
            new TokenTest(TokenConstants.SEMICOLON, ";"),
            new TokenTest(TokenConstants.RBRACE, "}"),
            new TokenTest(TokenConstants.ELSE, "else"),
            new TokenTest(TokenConstants.LBRACE, "{"),
            new TokenTest(TokenConstants.RETURN, "return"),
            new TokenTest(TokenConstants.FALSE, "false"),
            new TokenTest(TokenConstants.SEMICOLON, ";"),
            new TokenTest(TokenConstants.RBRACE, "}"),
            // 10 == 10;
            new TokenTest(TokenConstants.INT, "10"),
            new TokenTest(TokenConstants.EQ, "=="),
            new TokenTest(TokenConstants.INT, "10"),
            new TokenTest(TokenConstants.SEMICOLON, ";"),
		    // 10 != 9;
		    new TokenTest(TokenConstants.INT, "10"),
            new TokenTest(TokenConstants.NOT_EQ, "!="),
            new TokenTest(TokenConstants.INT, "9"),
            new TokenTest(TokenConstants.SEMICOLON, ";"),
		    // [1, 2];
		    new TokenTest(TokenConstants.LBRACKET, "["),
            new TokenTest(TokenConstants.INT, "1"),
            new TokenTest(TokenConstants.COMMA, ","),
            new TokenTest(TokenConstants.INT, "2"),
            new TokenTest(TokenConstants.RBRACKET, "]"),
            new TokenTest(TokenConstants.SEMICOLON, ";"),
            // "foo bar"
            // "foobar"
            new TokenTest(TokenConstants.STRING, "foo bar"),
            new TokenTest(TokenConstants.STRING, "foobar"),
            // {\"foo\": \"bar\"}
		    new TokenTest(TokenConstants.LBRACE, "{"),
            new TokenTest(TokenConstants.STRING, "foo"),
            new TokenTest(TokenConstants.COLON, ":"),
            new TokenTest(TokenConstants.STRING, "bar"),
            new TokenTest(TokenConstants.RBRACE, "}"),

            new TokenTest(TokenConstants.EOF, ""),
        };

        var lexer = new JoeLexer(input);

        for (int i = 0; i < tests.Length; i++) 
        { 
            var token = lexer.NextToken();

            Assert.Equal(tests[i].expectedType, token.Type);
            Assert.Equal(tests[i].expectedLiteral, token.Literal);
        }
    }
}