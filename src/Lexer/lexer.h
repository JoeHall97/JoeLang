#ifndef LEXER_H
#define LEXER_H
#include "../Tokens/token.h"

struct Lexer {
    int position;
    int readPosition;
    char *ch;
    int inputLength;
    char* input;
};
typedef struct Lexer Lexer;

Lexer create_lexer(char* input, int inputLength);
Token lexer_next_token(Lexer* l);
#endif