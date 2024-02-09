#pragma once

typedef enum {
    ILLEGAL,
    TOKEOF,
    // Identifiers & literal
    IDENT, // user defined identifiers. e.g. add, foobar, x, y, ...
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
} TokenType;

struct Token {
    TokenType type;
    char* literal;
	unsigned literal_length;
};
typedef struct Token Token;

// struct tokenTypeKeyValue {
// 	TokenType key;
// 	char* value;
// } tokenTypeMap[] = {
// 	ILLEGAL, "ILLEGAL",
//     TOKEOF, "EOF",
//     IDENT, "IDENTIFIER",
// 	INT, "INTEGER",
// 	STRING, "STRING",
// 	ASSIGN, "ASSIGN",
// 	PLUS, "PLUS",
// 	MINUS, "MIUNS",
// 	BANG, "BANG",
// 	ASTERISK, "ASTERISK",
// 	SLASH, "SLASH",
// 	LT, "LESS THAN",
// 	GT, "GREATER THAN",
// 	EQ, "EQUAL",
// 	NOT_EQ, "NOT EQUAL",
// 	COMMA, "COMMA",
// 	SEMICOLON, "SEMICOLON",
// 	LPAREN, "LEFT PARENTHESES",
// 	RPAREN, "RIGHT PARENTHESES",
// 	LBRACE, "LEFT BRACE",
// 	RBRACE, "RIGHT BRACE",
// 	LBRACKET, "LEFT BRACKET",
// 	RBRACKET, "RIGHT BRACKET",
// 	COLON, "COLON",
// 	FUNCTION, "FUNCTION",
// 	LET, "LET",
// 	TRUE, "TRUE",
// 	FALSE, "FALSE",
// 	IF, "IF",
// 	ELSE, "ELSE",
// 	RETURN, "RETURN",
// 	MACRO, "MACRO"
// };