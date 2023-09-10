using JoeLang.Lexer;
using JoeLang.Token;
using Newtonsoft.Json.Linq;
using System.Net.WebSockets;

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
		    new tokenTest(Tokens.VAR, "var"),
            new tokenTest(Tokens.IDENT, "five"),
            new tokenTest(Tokens.ASSIGN, "="),
            new tokenTest(Tokens.INT, "5"),
            new tokenTest(Tokens.SEMICOLON, ";"),
            // var ten = 10;
            new tokenTest(Tokens.VAR, "var"),
            new tokenTest(Tokens.IDENT, "ten"),
            new tokenTest(Tokens.ASSIGN, "=")   ,
            new tokenTest(Tokens.INT, "10"),
            new tokenTest(Tokens.SEMICOLON, ";"),
            // var add = fn(x, y) {
            // 		x + y;
            // };
            new tokenTest(Tokens.VAR, "var"),
            new tokenTest(Tokens.IDENT, "add"),
            new tokenTest(Tokens.ASSIGN, "="),
            new tokenTest(Tokens.FUNCTION, "fn"),
            new tokenTest(Tokens.LPAREN, "("),
            new tokenTest(Tokens.IDENT, "x"),
            new tokenTest(Tokens.COMMA, ","),
            new tokenTest(Tokens.IDENT, "y") ,
            new tokenTest(Tokens.RPAREN, ")"),
            new tokenTest(Tokens.LBRACE, "{"),
            new tokenTest(Tokens.IDENT, "x"),
            new tokenTest(Tokens.PLUS, "+"),
            new tokenTest(Tokens.IDENT, "y"),
            new tokenTest(Tokens.SEMICOLON, ";"),
            new tokenTest(Tokens.RBRACE, "}"),
            new tokenTest(Tokens.SEMICOLON, ";"),
            // var result = add(five, ten);
		    new tokenTest(Tokens.VAR, "var"),
            new tokenTest(Tokens.IDENT, "result"),
            new tokenTest(Tokens.ASSIGN, "="),
            new tokenTest(Tokens.IDENT, "add"),
            new tokenTest(Tokens.LPAREN, "("),
            new tokenTest(Tokens.IDENT, "five"),
            new tokenTest(Tokens.COMMA, ","),
            new tokenTest(Tokens.IDENT, "ten"),
            new tokenTest(Tokens.RPAREN, ")"),
            new tokenTest(Tokens.SEMICOLON, ";"),
            // !-/*5;
		    new tokenTest(Tokens.BANG, "!"),
            new tokenTest(Tokens.MINUS, "-"),
            new tokenTest(Tokens.SLASH, "/"),
            new tokenTest(Tokens.ASTERISK, "*"),
            new tokenTest(Tokens.INT, "5"),
            new tokenTest(Tokens.SEMICOLON, ";"),
            // 5 < 10 > 5;
            new tokenTest(Tokens.INT, "5"),
            new tokenTest(Tokens.LT, "<"),
            new tokenTest(Tokens.INT, "10"),
            new tokenTest(Tokens.GT, ">"),
            new tokenTest(Tokens.INT, "5"),
            new tokenTest(Tokens.SEMICOLON, ";"),
            // if (5 < 10) {
            // 		return true;
            // } else {
            // 		return false;
            // }
            new tokenTest(Tokens.IF, "if"),
            new tokenTest(Tokens.LPAREN, "("),
            new tokenTest(Tokens.INT, "5"),
            new tokenTest(Tokens.LT, "<"),
            new tokenTest(Tokens.INT, "10"),
            new tokenTest(Tokens.RPAREN, ")"),
            new tokenTest(Tokens.LBRACE, "{"),
            new tokenTest(Tokens.RETURN, "return"),
            new tokenTest(Tokens.TRUE, "true"),
            new tokenTest(Tokens.SEMICOLON, ";"),
            new tokenTest(Tokens.RBRACE, "}"),
            new tokenTest(Tokens.ELSE, "else"),
            new tokenTest(Tokens.LBRACE, "{"),
            new tokenTest(Tokens.RETURN, "return"),
            new tokenTest(Tokens.FALSE, "false"),
            new tokenTest(Tokens.SEMICOLON, ";"),
            new tokenTest(Tokens.RBRACE, "}"),
            // 10 == 10;
            new tokenTest(Tokens.INT, "10"),
            new tokenTest(Tokens.EQ, "=="),
            new tokenTest(Tokens.INT, "10"),
            new tokenTest(Tokens.SEMICOLON, ";"),
		    // 10 != 9;
		    new tokenTest(Tokens.INT, "10"),
            new tokenTest(Tokens.NOT_EQ, "!="),
            new tokenTest(Tokens.INT, "9"),
            new tokenTest(Tokens.SEMICOLON, ";"),

            new tokenTest(Tokens.EOF, ""),
        };

        var lexer = new JoeLexer(input);

        for (int i = 0; i < tests.Length; i++) 
        { 
            var token = lexer.nextToken();

            Assert.Equal(tests[i].expectedType, token.Type);
            Assert.Equal(tests[i].expectedLiteral, token.Literal);
        }
    }
}