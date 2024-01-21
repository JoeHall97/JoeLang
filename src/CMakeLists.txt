# CMakeList.txt : CMake project for JoeLang, include source and define
# project specific logic here.

#add_subdirectory(Lexer)
#add_subdirectory(AST)
#add_subdirectory(Tokens)

SET(SUB_FILES_SRC
  Lexer/lexer.c
  Tokens/token.c
)

SET(SUB_FILES_HDRS
  Lexer/lexer.h
  Tokens/token.c
)

# Add source to this project's executable.
add_executable (JoeLang "JoeLang.c" "JoeLang.h" ${SUB_FILES_SRC} ${SUB_FILES_HDRS})

if (CMAKE_VERSION VERSION_GREATER 3.21)
  set_property(TARGET JoeLang PROPERTY C_STANDARD 17)
else()
 set_property(TARGET JoeLang PROPERTY C_STANDARD 11)
endif()

# TODO: Add tests and install targets if needed.
