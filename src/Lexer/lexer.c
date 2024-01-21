#include <stdbool.h>
#include "lexer.h"

typedef struct string {
    char* string;
    int len;
} String;

void lexer_read_char(Lexer* l) {
    if (l->readPosition >= l->inputLength) {
        l->ch = 0;
    } else {
        l->ch = l->input[l->readPosition];
    }
    l->position = l->readPosition;
    l->readPosition++;
}

void lexer_skip_whitespace(Lexer* lexer) {
    while (lexer->ch == ' ' || lexer->ch == '\n' || lexer->ch == '\t' || 
        lexer->ch == '\r') {
        lexer_read_char(lexer);
    }
}

String lexer_strlen(Lexer* l) {
    char *p = l->input+l->readPosition;
}

Token lexer_next_token(Lexer* l) {
    Token tt;

    lexer_skip_whitespace(l);

    // switch (l->ch) {
    //     case '"':
    //         tt.type = STRING;
    //         String s = lexer_read_string(l);
    //         tt.literal = s.string;
    //         tt.literalLength = s.len;
    //         break;
    //     default:
    //         break;
    // }

    return tt;
}

char lexer_peek_char(Lexer* l) {
    return l->readPosition >= l->inputLength ? 0 : l->input[l->readPosition];
}


bool is_letter(char* c) {
    return (*c >= 'a' && *c <= 'z') || (*c >= 'A' && *c <= 'Z');
}

bool is_digit(char* c) {
    return *c >= '0' && *c <= '9';
}

Lexer create_lexer(char* input, int inputLength) {
    Lexer lexer;
    lexer.input = input;
    lexer.inputLength = inputLength;
    lexer_read_char(&lexer);
    return lexer;
}