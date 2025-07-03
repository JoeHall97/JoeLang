#include <ostream>
#include <token/token.hpp>

using token::TokenType;

const std::map<std::string, TokenType> keywords = {{"fn", TokenType::FUNCTION},
                                                   {"let", TokenType::LET},
                                                   {"true", TokenType::TRUE},
                                                   {"false", TokenType::FALSE},
                                                   {"if", TokenType::IF},
                                                   {"else", TokenType::ELSE},
                                                   {"return", TokenType::RETURN},
                                                   {
                                                           "macro",
                                                           TokenType::MACRO,
                                                   }};

TokenType token::lookUpIdent(const std::string &ident) {
    if (keywords.find(ident) != keywords.end())
        return keywords.at(ident);
    return TokenType::IDENT;
}

std::string token::tokenTypeToString(const TokenType &type) {
    if (token::tokenTypeMap.find(type) != token::tokenTypeMap.end())
        return token::tokenTypeMap.at(type);
    return "";
}

std::ostream &operator<<(std::ostream &os, const token::Token &token) {
    os << "{ ";
    os << token.literal << ", ";
    os << tokenTypeToString(token.type);
    os << " }";

    return os;
}
