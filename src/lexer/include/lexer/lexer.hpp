#pragma once
#include <string>
#include <token/token.hpp>

namespace lexer {
    class Lexer {
    public:
        explicit Lexer(std::string);
        token::Token nextToken();

    private:
        int position{};
        int readPosition;
        char ch{};
        std::string input;
        void skipWhitespace();
        void readChar();
        char peekChar() const;
        std::string readString();
        std::string readIdentifier();
        std::string readNumber();
    };
} // namespace lexer
