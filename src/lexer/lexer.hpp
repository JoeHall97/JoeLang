#pragma once
#include <string>
#include "tokens/tokens.hpp"

class Lexer {
    public:
        explicit Lexer (std::string);
        Token next_token();
    private:
        int position{};
        int readPosition;
        char ch{};
        std::string input;
        void skip_whitespace();
        void read_char();
        char peek_char();
        std::string read_string();
        std::string read_identifier();
        std::string read_number();
};
