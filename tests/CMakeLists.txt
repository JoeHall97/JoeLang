project(JoeLang_tests)
add_subdirectory(lib/googletest)
include_directories(${gtest_SOURCE_DIR}/include ${gtest_SOURCE_DIR})

add_executable(Google_Tests_run lexer_tests.cpp)
target_link_libraries(Google_Tests_run Lexer)
target_link_libraries(Google_Tests_run gtest gtest_main)