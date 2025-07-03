include(FetchContent)
FetchContent_Declare(
    googletest
    GIT_REPOSITORY https://github.com/google/googletest.git
    GIT_TAG v1.17.x
)
# this prevents overriding the parent project's compiler/linker setting on windows
set(gtest_force_shared_crt ON CACHE BOOL "" FORCE)
FetchContent_MakeAvailable(googletest)

add_executable(lexer_unit_tests
    "lexer_tests/lexer_tests.cpp")
target_link_libraries(lexer_unit_tests PRIVATE 
    gtest_main
    joelang::lexer)
include(GoogleTest)
gtest_discover_tests(lexer_unit_tests)
