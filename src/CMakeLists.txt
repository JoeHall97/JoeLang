add_subdirectory("lexer")
add_subdirectory("ast")
add_subdirectory("repl")

add_executable(JoeLang "main.cpp")

target_link_libraries(JoeLang PUBLIC
        joelang::repl)

set_property(TARGET JoeLang PROPERTY CXX_STANDARD 20)
