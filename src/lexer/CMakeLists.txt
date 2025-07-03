set(LEXER_SRC
        token.cpp
        lexer.cpp
)

set(LEXER_HDRS
        "./include/token/token.hpp"
        "./include/lexer/lexer.hpp"
)

add_library(lexer)

target_sources(lexer
	PUBLIC
		FILE_SET lexer_headers
		TYPE HEADERS
		BASE_DIRS ./include 
		FILES ${LEXER_HDRS}
	PRIVATE
		${LEXER_SRC}
)

add_library(joelang::lexer ALIAS lexer)