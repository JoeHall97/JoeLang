add_subdirectory("lexer")

add_executable(JoeLang "main.cpp")

target_link_libraries(JoeLang PUBLIC Lexer)

set_property(TARGET JoeLang PROPERTY CXX_STANDARD 20)