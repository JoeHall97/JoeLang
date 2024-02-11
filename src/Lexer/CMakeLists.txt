SET (LEXER_SRC
    lexer.c
    Tokens/tokens.c
)

SET (LEXER_HDRS
    lexer.h
    Tokens/tokens.h
)

ADD_LIBRARY(lexer ${LEXER_SRC} ${LEXER_HDRS})
