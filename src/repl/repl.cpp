#include <iostream>
#include <lexer/lexer.hpp>
#include <repl/repl.hpp>
#include <token/token.hpp>

void repl::REPLStart() {
    std::string line;
    while (true) {
        std::cout << "> ";
        std::getline(std::cin, line);
        if (line.empty())
            return;

        lexer::Lexer l{line};
        auto token = l.nextToken();
        while (token.type != token::TokenType::ENDOFFILE) {
            std::cout << token << '\n';
            token = l.nextToken();
        }
    }
}
