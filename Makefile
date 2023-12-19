build:
	clang++ -std=c++11 main.cpp lexer/lexer.cpp tokens/token.cpp -o bin/JoeLang
build_gcc:
	g++ -std=c++11 main.cpp lexer/lexer.cpp tokens/token.cpp -o bin/JoeLang
build_and_test:
	clang++ -std=c++11 tests.cpp lexer/lexer.cpp tokens/token.cpp -o bin/JoeLangTests
	./JoeLangTests
build_and_test_gcc:
	g++ -std=c++11 tests.cpp lexer/lexer.cpp tokens/token.cpp -o bin/JoeLangTests
	./JoeLangTests