# CMakeList.txt : CMake project for JoeLang, include source and define
# project specific logic here.

SET(TARGET_NAME JoeLang)

add_subdirectory("Lexer")

# Add source to this project's executable.
add_executable (${TARGET_NAME} "JoeLang.c" "JoeLang.h")

target_link_libraries(${TARGET_NAME} PUBLIC lexer)

if (CMAKE_VERSION VERSION_GREATER 3.21)
  set_property(TARGET ${TARGE_NAME} PROPERTY C_STANDARD 17)
else()
 set_property(TARGET ${TARGE_NAME} PROPERTY C_STANDARD 11)
endif()

# TODO: Add tests and install targets if needed.
