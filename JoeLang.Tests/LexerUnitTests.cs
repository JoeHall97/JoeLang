using JoeLang.Lexer;
using JoeLang.Constants;

namespace JoeLang.Tests;

public class LexerUnitTests
{
	private struct tokenTest
	{
        public tokenTest(string type, string literal)
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
		10 != 9;";

		var tests = new tokenTest[]
        {
            // var five = 5;
		    new tokenTest(TokenConstants.VAR, "var"),
            new tokenTest(TokenConstants.IDENT, "five"),
            new tokenTest(TokenConstants.ASSIGN, "="),
            new tokenTest(TokenConstants.INT, "5"),
            new tokenTest(TokenConstants.SEMICOLON, ";"),
            // var ten = 10;
            new tokenTest(TokenConstants.VAR, "var"),
            new tokenTest(TokenConstants.IDENT, "ten"),
            new tokenTest(TokenConstants.ASSIGN, "=")   ,
            new tokenTest(TokenConstants.INT, "10"),
            new tokenTest(TokenConstants.SEMICOLON, ";"),
            // var add = fn(x, y) {
            // 		x + y;
            // };
            new tokenTest(TokenConstants.VAR, "var"),
            new tokenTest(TokenConstants.IDENT, "add"),
            new tokenTest(TokenConstants.ASSIGN, "="),
            new tokenTest(TokenConstants.FUNCTION, "fn"),
            new tokenTest(TokenConstants.LPAREN, "("),
            new tokenTest(TokenConstants.IDENT, "x"),
            new tokenTest(TokenConstants.COMMA, ","),
            new tokenTest(TokenConstants.IDENT, "y") ,
            new tokenTest(TokenConstants.RPAREN, ")"),
            new tokenTest(TokenConstants.LBRACE, "{"),
            new tokenTest(TokenConstants.IDENT, "x"),
            new tokenTest(TokenConstants.PLUS, "+"),
            new tokenTest(TokenConstants.IDENT, "y"),
            new tokenTest(TokenConstants.SEMICOLON, ";"),
            new tokenTest(TokenConstants.RBRACE, "}"),
            new tokenTest(TokenConstants.SEMICOLON, ";"),
            // var result = add(five, ten);
		    new tokenTest(TokenConstants.VAR, "var"),
            new tokenTest(TokenConstants.IDENT, "result"),
            new tokenTest(TokenConstants.ASSIGN, "="),
            new tokenTest(TokenConstants.IDENT, "add"),
            new tokenTest(TokenConstants.LPAREN, "("),
            new tokenTest(TokenConstants.IDENT, "five"),
            new tokenTest(TokenConstants.COMMA, ","),
            new tokenTest(TokenConstants.IDENT, "ten"),
            new tokenTest(TokenConstants.RPAREN, ")"),
            new tokenTest(TokenConstants.SEMICOLON, ";"),
            // !-/*5;
		    new tokenTest(TokenConstants.BANG, "!"),
            new tokenTest(TokenConstants.MINUS, "-"),
            new tokenTest(TokenConstants.SLASH, "/"),
            new tokenTest(TokenConstants.ASTERISK, "*"),
            new tokenTest(TokenConstants.INT, "5"),
            new tokenTest(TokenConstants.SEMICOLON, ";"),
            // 5 < 10 > 5;
            new tokenTest(TokenConstants.INT, "5"),
            new tokenTest(TokenConstants.LT, "<"),
            new tokenTest(TokenConstants.INT, "10"),
            new tokenTest(TokenConstants.GT, ">"),
            new tokenTest(TokenConstants.INT, "5"),
            new tokenTest(TokenConstants.SEMICOLON, ";"),
            // if (5 < 10) {
            // 		return true;
            // } else {
            // 		return false;
            // }
            new tokenTest(TokenConstants.IF, "if"),
            new tokenTest(TokenConstants.LPAREN, "("),
            new tokenTest(TokenConstants.INT, "5"),
            new tokenTest(TokenConstants.LT, "<"),
            new tokenTest(TokenConstants.INT, "10"),
            new tokenTest(TokenConstants.RPAREN, ")"),
            new tokenTest(TokenConstants.LBRACE, "{"),
            new tokenTest(TokenConstants.RETURN, "return"),
            new tokenTest(TokenConstants.TRUE, "true"),
            new tokenTest(TokenConstants.SEMICOLON, ";"),
            new tokenTest(TokenConstants.RBRACE, "}"),
            new tokenTest(TokenConstants.ELSE, "else"),
            new tokenTest(TokenConstants.LBRACE, "{"),
            new tokenTest(TokenConstants.RETURN, "return"),
            new tokenTest(TokenConstants.FALSE, "false"),
            new tokenTest(TokenConstants.SEMICOLON, ";"),
            new tokenTest(TokenConstants.RBRACE, "}"),
            // 10 == 10;
            new tokenTest(TokenConstants.INT, "10"),
            new tokenTest(TokenConstants.EQ, "=="),
            new tokenTest(TokenConstants.INT, "10"),
            new tokenTest(TokenConstants.SEMICOLON, ";"),
		    // 10 != 9;
		    new tokenTest(TokenConstants.INT, "10"),
            new tokenTest(TokenConstants.NOT_EQ, "!="),
            new tokenTest(TokenConstants.INT, "9"),
            new tokenTest(TokenConstants.SEMICOLON, ";"),

            new tokenTest(TokenConstants.EOF, ""),
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