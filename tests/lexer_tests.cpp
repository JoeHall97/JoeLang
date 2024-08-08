#include <gtest/gtest.h>
#include <string>
#include <iostream>
#include "../src/Lexer/lexer.hpp"

struct lexer_tests {
	TokenType expectedType;
	std::string expectedLiteral;
};

TEST(LexerTestSuite, NextToken) {
    const std::string input =
    	"let five = 5;\n"
		"let ten = 10;\n\n"

		"let add = fn(x, y) {\n"
		"	x + y;\n"
		"};\n\n"

		"let result = add(five, ten);\n"
		"!-/*5;\n"
		"5 < 10 > 5;\n\n"

		"if (5 < 10) {\n"
		"	return true;\n"
		"} else {\n"
		"	return false;\n"
		"}\n\n"

		"10 == 10;\n"
		"10 != 9;\n";
    const std::vector<lexer_tests> tests = {
	    // let five = 5;
	    {LET, "let"},
	    {IDENT, "five"},
	    {ASSIGN, "="},
	    {INT, "5"},
	    {SEMICOLON, ";"},
	    // let ten = 10;
	    {LET, "let"},
	    {IDENT, "ten"},
	    {ASSIGN, "="},
	    {INT, "10"},
	    {SEMICOLON, ";"},
	    // let add = fn(x, y) {
	    //	x + y;
	    //};
	    {LET, "let"},
	    {IDENT, "add"},
	    {ASSIGN, "="},
	    {FUNCTION, "fn"},
	    {LPAREN, "("},
	    {IDENT, "x"},
	    {COMMA, ","},
	    {IDENT, "y"},
	    {RPAREN, ")"},
	    {LBRACE, "{"},
	    {IDENT, "x"},
	    {PLUS, "+"},
	    {IDENT, "y"},
	    {SEMICOLON, ";"},
	    {RBRACE, "}"},
	    {SEMICOLON, ";"},
	    // let result = add(five, ten);
	    {LET, "let"},
	    {IDENT, "result"},
	    {ASSIGN, "="},
	    {IDENT, "add"},
	    {LPAREN, "("},
	    {IDENT, "five"},
	    {COMMA, ","},
	    {IDENT, "ten"},
	    {RPAREN, ")"},
	    {SEMICOLON, ";"},
	    // !-/*5;
	    {BANG, "!"},
	    {MINUS, "-"},
	    {SLASH, "/"},
	    {ASTERISK, "*"},
	    {INT, "5"},
	    {SEMICOLON, ";"},
	    // 5 < 10 > 5;
	    {INT, "5"},
	    {LT, "<"},
	    {INT, "10"},
	    {GT, ">"},
	    {INT, "5"},
	    {SEMICOLON, ";"},
	    // if (5 < 10) {
	    // 		return true;
	    // } else {
	    // 		return false;
	    // }
	    {IF, "if"},
	    {LPAREN, "("},
	    {INT, "5"},
	    {LT, "<"},
	    {INT, "10"},
	    {RPAREN, ")"},
	    {LBRACE, "{"},
	    {RETURN, "return"},
	    {TRUE, "true"},
	    {SEMICOLON, ";"},
	    {RBRACE, "}"},
	    {ELSE, "else"},
	    {LBRACE, "{"},
	    {RETURN, "return"},
	    {FALSE, "false"},
	    {SEMICOLON, ";"},
	    {RBRACE, "}"},
	    // 10 == 10;
	    {INT, "10"},
	    {EQ, "=="},
	    {INT, "10"},
	    {SEMICOLON, ";"},
	    // 10 != 9;
	    {INT, "10"},
	    {NOT_EQ, "!="},
	    {INT, "9"},
	    {SEMICOLON, ";"},

	    {ENDOFFILE, ""},
    };


    Lexer lexer (input);

	for (const auto& [expectedType, expectedLiteral] : tests) {
		const auto& [type, literal] = lexer.next_token();
		ASSERT_EQ(type, expectedType);
		ASSERT_EQ(literal, expectedLiteral);
	}
}
