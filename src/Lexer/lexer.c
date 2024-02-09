#include <stdbool.h>
#include "lexer.h"

#define PEEK_CHAR(l) (l->read_position >= l->input_length ? 0 : l->input[l->read_position])
#define IS_LETTER(ch) ((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z'))
#define IS_DIGIT(ch) (ch >= '0' && ch <= '9')
#define SKIP_WHITESPACE(l) \
while(*l->ch == ' ' || *l->ch == '\n' || *l->ch == '\t' || *l->ch == '\r') \
{ \
    LexerReadChar(l); \
}

void LexerReadChar(Lexer* l) {
    if (l->read_position >= l->input_length) {
        l->ch = 0;
    } else {
        l->ch = l->input[l->read_position];
    }
    l->position = l->read_position;
    l->read_position++;
}

int LexerStrlen(Lexer* l) {
    int cnt = 0;

    for ( ; *l->ch != '"' || *l->ch != '\0'; cnt++)
        LexerReadChar(l);

    return cnt;
}

Token LexerNextToken(Lexer* l) {
    Token t;

    SKIP_WHITESPACE(l);

    switch (*l->ch) {
        case '"':
            t.type = STRING;
            t.literal_length = LexerStrlen(l);
            t.literal = l->ch-t.literal_length;
            break;
        case '=':
            if (PEEK_CHAR(l) == '=') {
                t.type = EQ;
                t.literal = l->ch;
                t.literal_length = 2;
                LexerReadChar(l);
                break;
            }
            t.type = ASSIGN;
            t.literal = l->ch;
            t.literal_length = 1;
            break;
        case '+':
            t.type = PLUS;
            t.literal = l->ch;
            t.literal_length = 1;
            break;
        case '-':
            t.type = MINUS;
            t.literal = l->ch;
            t.literal_length = 1;
            break;
        case '!':
            if (PEEK_CHAR(l) == '=') {
                t.type = NOT_EQ;
                t.literal = l->ch;
                t.literal_length = 2;
                LexerReadChar(l);
                break;
            }
            t.type = BANG;
            t.literal = l->ch;
            t.literal_length = 1;
            break;
        case '/':
            t.type = SLASH;
            t.literal = l->ch;
            t.literal_length = 1;
            break;
        case '*':
            t.type = ASTERISK;
            t.literal = l->ch;
            t.literal_length = 1;
            break;
        case '<':
            t.type = LT;
            t.literal = l->ch;
            t.literal_length = 1;
            break;
        case '>':
            t.type = GT;
            t.literal = l->ch;
            t.literal_length = 1;
            break;
        case ';':
            t.type = SEMICOLON;
            t.literal = l->ch;
            t.literal_length = 1;
            break;
        case ',':
            t.type = COMMA;
            t.literal = l->ch;
            t.literal_length = 1;
            break;
        case '(':
            t.type = LPAREN;
            t.literal = l->ch;
            t.literal_length = 1;
            break;
        case ')':
            t.type = RPAREN;
            t.literal = l->ch;
            t.literal_length = 1;
            break;
        case '{':
            t.type = LBRACE;
            t.literal = l->ch;
            t.literal_length = 1;
            break;
        case '}':
            t.type = RBRACE;
            t.literal = l->ch;
            t.literal_length = 1;
            break;
        case '[':
            t.type = LBRACKET;
            t.literal = l->ch;
            t.literal_length = 1;
            break;
        case ']':
            t.type = RBRACKET;
            t.literal = l->ch;
            t.literal_length = 1;
            break;
        case ':':
            t.type = COLON;
            t.literal = l->ch;
            t.literal_length = 1;
            break;
        case '\0':
            t.type = TOKEOF;
            t.literal = l->ch;
            t.literal_length = 1;
            break;
        default:
            if (IS_LETTER(*l->ch)) {
                // ...
                return t;
            } else if (IS_DIGIT(*l->ch)) {
                // ...
                return t;
            }
            t.type = ILLEGAL;
            t.literal = l->ch;
            t.literal_length = 1;
            break;
    }

    LexerReadChar(l);
    return t;
}

Lexer CreateLexer(char* input, int input_length) {
    Lexer lexer;
    lexer.input = input;
    lexer.input_length = input_length;
    LexerReadChar(&lexer);
    return lexer;
}