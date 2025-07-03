#pragma once

#include <map>
#include <ostream>
#include <string>

namespace token {
    enum class TokenType {
        ILLEGAL,
        ENDOFFILE,
        // Identifiers + literals
        IDENT,
        INT,
        STRING,
        // Operators
        ASSIGN,
        PLUS,
        MINUS,
        BANG,
        ASTERISK,
        SLASH,
        LT,
        GT,
        EQ,
        NOT_EQ,
        // Delimiters
        COMMA,
        SEMICOLON,
        LPAREN,
        RPAREN,
        LBRACE,
        RBRACE,
        LBRACKET,
        RBRACKET,
        COLON,
        // Keywords
        FUNCTION,
        LET,
        TRUE,
        FALSE,
        IF,
        ELSE,
        RETURN,
        MACRO
    };

    const std::map<TokenType, std::string> tokenTypeMap = {
            {TokenType::ILLEGAL, "ILLEGAL"},
            {TokenType::ENDOFFILE, "EOF"},
            {TokenType::IDENT, "IDENTIFIER"},
            {TokenType::INT, "INTEGER"},
            {TokenType::STRING, "STRING"},
            {TokenType::ASSIGN, "ASSIGN"},
            {TokenType::PLUS, "PLUS"},
            {TokenType::MINUS, "MINUS"},
            {TokenType::BANG, "BANG"},
            {TokenType::ASTERISK, "ASTERISK"},
            {TokenType::SLASH, "SLASH"},
            {TokenType::LT, "LESS THAN"},
            {TokenType::GT, "GREATER THAN"},
            {TokenType::EQ, "EQUALS"},
            {TokenType::NOT_EQ, "NOT EQUALS"},
            {TokenType::COMMA, "COMMA"},
            {TokenType::SEMICOLON, "SEMICOLON"},
            {TokenType::LPAREN, "LEFT PARENTHESES"},
            {TokenType::RPAREN, "RIGHT PARENTHESES"},
            {TokenType::LBRACE, "LEFT BRACE"},
            {TokenType::RBRACE, "RIGHT BRACE"},
            {TokenType::LBRACKET, "LEFT BRACKET"},
            {TokenType::RBRACKET, "RIGHT BRACKET"},
            {TokenType::COLON, "COLON"},
            {TokenType::FUNCTION, "FUNCTION"},
            {TokenType::LET, "LET"},
            {TokenType::TRUE, "TRUE"},
            {TokenType::FALSE, "IDENTIFIER"},
            {TokenType::IF, "IF"},
            {TokenType::ELSE, "ELSE"},
            {TokenType::RETURN, "RETURN"},
            {TokenType::MACRO, "MACRO"},
    };

    struct Token {
        std::string literal;
        TokenType type;
    };

    TokenType lookUpIdent(const std::string &ident);
    std::string tokenTypeToString(const TokenType &type);
} // namespace token

std::ostream &operator<<(std::ostream &os, const token::Token &token);
