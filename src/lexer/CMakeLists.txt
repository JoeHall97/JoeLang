SET (LEXER_SRC
    lexer.cpp
    tokens/tokens.cpp
)

SET (LEXER_HDRS
    lexer.hpp
    tokens/tokens.hpp
)

ADD_LIBRARY(Lexer STATIC ${LEXER_SRC} ${LEXER_SRC})