#pragma once

#include "Tokens/tokens.h"

struct Lexer {
    unsigned position;
    unsigned read_position;
    char *ch;
    unsigned input_length;
    char *input;
};
typedef struct Lexer Lexer;

Lexer createlexer(char *input, int input_length);
Token nexttoken(Lexer *l);