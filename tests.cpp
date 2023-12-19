/// NOTE: need to actually use a unit test framework instead. 
/// Have to figure out how to set it up with cmake though...
#include <string>
#include <iostream>
#include "lexer/lexer.hpp"

struct lexerTest {
    TokenType tt;
    std::string literal;
};

bool test_lexer() {
    auto input = "let five = 5; \
	let ten = 10; \
	let add = fn(x, y) { \
		x + y; \
	}; \
	let result = add(five, ten); \
	!-/*5; \
	5 < 10 > 5; \
	if (5 < 10) { \
		return true; \
	} else { \
		return false; \
	} \
    10 == 10; \
    10 != 9; \
	\"foo bar\" \
	\"foobar\" \
	[1, 2]; \
	{\"foo\": \"bar\"} \
	macro(x, y) { x + y; };";

    lexerTest tests[] = {
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
		// 		x + y;
		// };
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
		// "foo bar"
		// "foobar"
		{STRING, "foo bar"},
		{STRING, "foobar"},
		// [1, 2];
		{LBRACKET, "["},
		{INT, "1"},
		{COMMA, ","},
		{INT, "2"},
		{RBRACKET, "]"},
		{SEMICOLON, ";"},
		// {"foo": "bar"}
		{LBRACE, "{"},
		{STRING, "foo"},
		{COLON, ":"},
		{STRING, "bar"},
		{RBRACE, "}"},
		// macro(x, y) { x + y; };
		{MACRO, "macro"},
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

		{ENDOFFILE, ""}
    };

    Lexer l (input);
    int testNum = 0;

    for (auto t: tests) {
        auto token = l.next_token();
        
        if (t.tt != token.type) {
            std::cout << "TEST " << testNum << " FAILED: ";
            std::cout << "expected token type=" << token_type_to_string(t.tt);
            std::cout << ", got=" << token_type_to_string(token.type) << std::endl;
            return false; 
        }

        if (t.literal != token.literal) {
            std::cout << "TEST " << testNum << " FAILED: ";
            std::cout << "expected literal=" << t.literal;
            std::cout << ", got=" << token.literal << std::endl;
            return false; 
        }

        ++testNum;
    }

    return true;
}

int main() {
    bool res = test_lexer();
    return res ? 0 : 1;
}
