#include <ctype.h>
#include "lexer.hpp"
#include <iostream>

// public Lexer functions

Lexer::Lexer(std::string in) {
    input = in;
    readPosition = 0;
    read_char();
}

Token Lexer::next_token() {
    Token token;

    skip_whitespace();

    switch (ch) {
        case '"':
            token.type = STRING;
            token.literal = read_string();
            break;
        case '=':
            if (peek_char() == '=') {
                auto prev_ch = std::string(1,ch);
                read_char();
                token.type = EQ;
                token.literal = prev_ch + ch;
                break;
            }
            token.type = ASSIGN;
            token.literal = ch;
            break;
        case '+':
            token.type = PLUS;
            token.literal = ch;
            break;
        case '-':
            token.type = MINUS;
            token.literal = ch;
            break;
        case '!':
            if (peek_char() == '=') {
                auto prev_ch = std::string(1,ch);
                read_char();
                token.type = NOT_EQ;
                token.literal = prev_ch + ch;
                break;
            }
            token.type = BANG;
            token.literal = ch;
            break;
        case '/':
            token.type = SLASH;
            token.literal = ch;
            break;
        case '*':
            token.type = ASTERISK;
            token.literal = ch;
            break;
        case '<':
            token.type = LT;
            token.literal = ch;
            break;
        case '>':
            token.type = GT;
            token.literal = ch;
            break;
        case ';':
            token.type = SEMICOLON;
            token.literal = ch;
            break;
        case ',':
            token.type = COMMA;
            token.literal = ch;
            break;
        case '(':
            token.type = LPAREN;
            token.literal = ch;
            break;
        case ')':
            token.type = RPAREN;
            token.literal = ch;
            break;
        case '{':
            token.type = LBRACE;
            token.literal = ch;
            break;
        case '}':
            token.type = RBRACE;
            token.literal = ch;
            break;
        case '[':
            token.type = LBRACKET;
            token.literal = ch;
            break;
        case ']':
            token.type = RBRACKET;
            token.literal = ch;
            break;
        case ':':
            token.type = COLON;
            token.literal = ch;
            break;
        case '\0':
            token.type = ENDOFFILE;
            token.literal = "";
            break;
        default:
            if (isalpha(ch)) {
                token.literal = read_identifier();
                token.type = look_up_ident(token.literal);
                return token;
            } else if (isdigit(ch)) {
                token.type = INT;
                token.literal = read_number();
                return token;
            }
            token.type = ILLEGAL;
            token.literal = ch;
            break;
    }

    read_char();
    return token;
}
 
// private Lexer functions

std::string Lexer::read_string() {
    int pos = position + 1;
    int length = 0;

    while (true) {
        read_char();
        ++length;
        if (ch == '"' || ch == '\0')
            break;
    }

    return input.substr(pos, length-1);
}

std::string Lexer::read_identifier() {
    int pos = position;
    int length = 0;

    while (isalpha(ch) && ch != '"') {
        read_char();
        ++length;
    }

    return input.substr(pos, length);
}

std::string Lexer::read_number() {
    int pos = position;
    int length = 0;

    while (isdigit(ch)) {
        read_char();
        ++length;
    }

    return input.substr(pos, length);
}

void Lexer::skip_whitespace() {
    while (isspace(ch)) {
        read_char();
    }
}

void Lexer::read_char() {
    if (readPosition >= input.length()) {
        ch = '\0';
    } else {
        ch = input[readPosition];
    }
    position = readPosition;
    ++readPosition;
}

char Lexer::peek_char() {
    return readPosition > input.length() ? '\0' : input[readPosition];
}