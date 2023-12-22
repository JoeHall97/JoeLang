build:
	clang++ -std=c++17 main.cpp lexer/lexer.cpp tokens/token.cpp ast/ast.cpp -o bin/JoeLang
build_gcc:
	g++ -std=c++17 main.cpp lexer/lexer.cpp tokens/token.cpp ast/ast.cpp -o bin/JoeLang
build_and_test:
	clang++ -std=c++17 tests.cpp lexer/lexer.cpp tokens/token.cpp ast/ast.cpp -o bin/JoeLangTests
	./JoeLangTests
build_and_test_gcc:
	g++ -std=c++17 tests.cpp lexer/lexer.cpp tokens/token.cpp ast/ast.cpp -o bin/JoeLangTests
	./JoeLangTests