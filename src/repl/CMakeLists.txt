add_library(repl)

target_sources(repl
	PUBLIC
		FILE_SET repl_headers
		TYPE HEADERS
		BASE_DIRS ./include 
		FILES ./include/repl/repl.hpp
	PRIVATE
		repl.cpp
)

target_link_libraries(repl PRIVATE
	joelang::lexer
	joelang::ast)

add_library(joelang::repl ALIAS repl)
