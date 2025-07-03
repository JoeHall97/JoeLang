#include <lexer/lexer.hpp>
#include <token/token.hpp>

using lexer::Lexer, token::Token, token::TokenType;

Lexer::Lexer(std::string in) {
    input = in;
    readPosition = 0;
    readChar();
}

Token Lexer::nextToken() {
    Token token;

    skipWhitespace();

    switch (ch) {
        case '"':
            token.type = TokenType::STRING;
            token.literal = readString();
            break;
        case '=':
            if (peekChar() == '=') {
                auto prev_ch = std::string(1, ch);
                readChar();
                token.type = TokenType::EQ;
                token.literal = prev_ch + ch;
                break;
            }
            token.type = TokenType::ASSIGN;
            token.literal = ch;
            break;
        case '+':
            token.type = TokenType::PLUS;
            token.literal = ch;
            break;
        case '-':
            token.type = TokenType::MINUS;
            token.literal = ch;
            break;
        case '!':
            if (peekChar() == '=') {
                auto prev_ch = std::string(1, ch);
                readChar();
                token.type = TokenType::NOT_EQ;
                token.literal = prev_ch + ch;
                break;
            }
            token.type = TokenType::BANG;
            token.literal = ch;
            break;
        case '/':
            token.type = TokenType::SLASH;
            token.literal = ch;
            break;
        case '*':
            token.type = TokenType::ASTERISK;
            token.literal = ch;
            break;
        case '<':
            token.type = TokenType::LT;
            token.literal = ch;
            break;
        case '>':
            token.type = TokenType::GT;
            token.literal = ch;
            break;
        case ';':
            token.type = TokenType::SEMICOLON;
            token.literal = ch;
            break;
        case ',':
            token.type = TokenType::COMMA;
            token.literal = ch;
            break;
        case '(':
            token.type = TokenType::LPAREN;
            token.literal = ch;
            break;
        case ')':
            token.type = TokenType::RPAREN;
            token.literal = ch;
            break;
        case '{':
            token.type = TokenType::LBRACE;
            token.literal = ch;
            break;
        case '}':
            token.type = TokenType::RBRACE;
            token.literal = ch;
            break;
        case '[':
            token.type = TokenType::LBRACKET;
            token.literal = ch;
            break;
        case ']':
            token.type = TokenType::RBRACKET;
            token.literal = ch;
            break;
        case ':':
            token.type = TokenType::COLON;
            token.literal = ch;
            break;
        case '\0':
            token.type = TokenType::ENDOFFILE;
            token.literal = "";
            break;
        default:
            if (isalpha(ch)) {
                token.literal = readIdentifier();
                token.type = token::lookUpIdent(token.literal);
                return token;
            }
            if (isdigit(ch)) {
                token.type = TokenType::INT;
                token.literal = readNumber();
                return token;
            }
            token.type = TokenType::ILLEGAL;
            token.literal = ch;
            break;
    }

    readChar();
    return token;
}

std::string Lexer::readString() {
    int pos = position + 1;
    int length = 0;

    while (true) {
        readChar();
        ++length;
        if (ch == '"' || ch == '\0')
            break;
    }

    return input.substr(pos, length - 1);
}

std::string Lexer::readIdentifier() {
    int pos = position;
    int length = 0;

    while (isalpha(ch) && ch != '"') {
        readChar();
        ++length;
    }

    return input.substr(pos, length);
}

std::string Lexer::readNumber() {
    int pos = position;
    int length = 0;

    while (isdigit(ch)) {
        readChar();
        ++length;
    }

    return input.substr(pos, length);
}

void Lexer::skipWhitespace() {
    while (isspace(ch)) {
        readChar();
    }
}

void Lexer::readChar() {
    if (readPosition >= input.length()) {
        ch = '\0';
    } else {
        ch = input[readPosition];
    }
    position = readPosition;
    ++readPosition;
}

char Lexer::peekChar() const {
    return readPosition > input.length() ? '\0' : input[readPosition];
}
