#include <gtest/gtest.h>
#include <lexer/lexer.hpp>
#include <string>
#include <token/token.hpp>

using token::TokenType;

struct TestExpect {
    TokenType expectedType;
    std::string expectedLiteral;
};

TEST(LexerTestSuite, NextToken) {
    const std::string input = "let five = 5;\n"
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
    const std::vector<TestExpect> tests = {
            // let five = 5;
            {TokenType::LET, "let"},
            {TokenType::IDENT, "five"},
            {TokenType::ASSIGN, "="},
            {TokenType::INT, "5"},
            {TokenType::SEMICOLON, ";"},
            // let ten = 10;
            {TokenType::LET, "let"},
            {TokenType::IDENT, "ten"},
            {TokenType::ASSIGN, "="},
            {TokenType::INT, "10"},
            {TokenType::SEMICOLON, ";"},
            // let add = fn(x, y) {
            //	x + y;
            //};
            {TokenType::LET, "let"},
            {TokenType::IDENT, "add"},
            {TokenType::ASSIGN, "="},
            {TokenType::FUNCTION, "fn"},
            {TokenType::LPAREN, "("},
            {TokenType::IDENT, "x"},
            {TokenType::COMMA, ","},
            {TokenType::IDENT, "y"},
            {TokenType::RPAREN, ")"},
            {TokenType::LBRACE, "{"},
            {TokenType::IDENT, "x"},
            {TokenType::PLUS, "+"},
            {TokenType::IDENT, "y"},
            {TokenType::SEMICOLON, ";"},
            {TokenType::RBRACE, "}"},
            {TokenType::SEMICOLON, ";"},
            // let result = add(five, ten);
            {TokenType::LET, "let"},
            {TokenType::IDENT, "result"},
            {TokenType::ASSIGN, "="},
            {TokenType::IDENT, "add"},
            {TokenType::LPAREN, "("},
            {TokenType::IDENT, "five"},
            {TokenType::COMMA, ","},
            {TokenType::IDENT, "ten"},
            {TokenType::RPAREN, ")"},
            {TokenType::SEMICOLON, ";"},
            // !-/*5;
            {TokenType::BANG, "!"},
            {TokenType::MINUS, "-"},
            {TokenType::SLASH, "/"},
            {TokenType::ASTERISK, "*"},
            {TokenType::INT, "5"},
            {TokenType::SEMICOLON, ";"},
            // 5 < 10 > 5;
            {TokenType::INT, "5"},
            {TokenType::LT, "<"},
            {TokenType::INT, "10"},
            {TokenType::GT, ">"},
            {TokenType::INT, "5"},
            {TokenType::SEMICOLON, ";"},
            // if (5 < 10) {
            // 		return true;
            // } else {
            // 		return false;
            // }
            {TokenType::IF, "if"},
            {TokenType::LPAREN, "("},
            {TokenType::INT, "5"},
            {TokenType::LT, "<"},
            {TokenType::INT, "10"},
            {TokenType::RPAREN, ")"},
            {TokenType::LBRACE, "{"},
            {TokenType::RETURN, "return"},
            {TokenType::TRUE, "true"},
            {TokenType::SEMICOLON, ";"},
            {TokenType::RBRACE, "}"},
            {TokenType::ELSE, "else"},
            {TokenType::LBRACE, "{"},
            {TokenType::RETURN, "return"},
            {TokenType::FALSE, "false"},
            {TokenType::SEMICOLON, ";"},
            {TokenType::RBRACE, "}"},
            // 10 == 10;
            {TokenType::INT, "10"},
            {TokenType::EQ, "=="},
            {TokenType::INT, "10"},
            {TokenType::SEMICOLON, ";"},
            // 10 != 9;
            {TokenType::INT, "10"},
            {TokenType::NOT_EQ, "!="},
            {TokenType::INT, "9"},
            {TokenType::SEMICOLON, ";"},

            {TokenType::ENDOFFILE, ""},
    };

    lexer::Lexer lexer(input);

    for (const auto &[expectedType, expectedLiteral]: tests) {
        const auto &[literal, type] = lexer.nextToken();
        ASSERT_EQ(type, expectedType);
        ASSERT_EQ(literal, expectedLiteral);
    }
}
