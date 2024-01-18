# CMakeList.txt : CMake project for JoeLang, include source and define
# project specific logic here.
#

# Add source to this project's executable.
add_executable (JoeLang "JoeLang.c" "JoeLang.h")

if (CMAKE_VERSION VERSION_GREATER 3.21)
  set_property(TARGET JoeLang PROPERTY C_STANDARD 17)
else()
 set_property(TARGET JoeLang PROPERTY C_STANDARD 11)
endif()

# TODO: Add tests and install targets if needed.
