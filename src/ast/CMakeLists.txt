set(AST_HDRS
        "./include/ast/ast.hpp")

set(AST_SRC
        "ast.cpp")

add_library(ast)

target_sources(ast
	PUBLIC
		FILE_SET ast_headers
		TYPE HEADERS
		BASE_DIRS ./include 
		FILES ${AST_HDRS}
	PRIVATE
		${AST_SRC}
)

target_link_libraries(ast PRIVATE joelang::lexer)

add_library(joelang::ast ALIAS ast)
