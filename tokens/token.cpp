#include "token.hpp"

const std::map<std::string, TokenType> keywords = {
    {"fn", FUNCTION},
	{"let", LET},
	{"true", TRUE},
	{"false", FALSE},
	{"if", IF},
	{"else", ELSE},
	{"return", RETURN},
	{"macro",  MACRO,}
};

TokenType look_up_ident(std::string ident) {
    if (keywords.find(ident) != keywords.end())
        return keywords.at(ident);
    return IDENT;
}

std::string token_type_to_string(TokenType token) {
    if (tokenTypeMap.find(token) != tokenTypeMap.end())
        return tokenTypeMap.at(token);
    return "";
}