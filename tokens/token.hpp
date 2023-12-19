#ifndef TOKEN_H
#define TOKEN_H
#include <string>
#include <map>

enum TokenType {
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
    {ILLEGAL, "ILLEGAL"},
    {ENDOFFILE, "EOF"},
    {IDENT, "IDENTIFIER"},
    {INT, "INTEGER"},
    {STRING, "STRING"},
    {ASSIGN, "ASSIGN"},
    {PLUS, "PLUS"},
    {MINUS, "MINUS"},
    {BANG, "BANG"},
    {ASTERISK, "ASTERISK"},
    {SLASH, "SLASH"},
    {LT, "LESS THAN"},
    {GT, "GREATER THAN"},
    {EQ, "EQUALS"},
    {NOT_EQ, "NOT EQUALS"},
    {COMMA, "COMMA"},
    {SEMICOLON, "SEMICOLON"},
    {LPAREN, "LEFT PARENTHESES"},
    {RPAREN, "RIGHT PARENTHESES"},
    {LBRACE, "LEFT BRACE"},
    {RBRACE, "RIGHT BRACE"},
    {LBRACKET, "LEFT BRACKET"},
    {RBRACKET, "RIGHT BRACKET"},
    {COLON, "COLON"},
    {FUNCTION, "FUNCTION"},
    {LET, "LET"},
    {TRUE, "TRUE"},
    {FALSE, "IDENTIFIER"},
    {IF, "IF"},
    {ELSE, "ELSE"},
    {RETURN, "RETURN"},
    {MACRO, "MACRO"},
};

struct Token {
    TokenType type;
    std::string literal;
};

#endif

TokenType look_up_ident(std::string);
std::string token_type_to_string(TokenType);