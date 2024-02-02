#include <iostream>
#include "lexer/lexer.hpp"

int main() {
    Lexer lexer ("let a = 0;");
    auto token = lexer.next_token();
    
    // while (token.type != ENDOFFILE) {
    //     std::cout << "type: " << token_type_to_string(token.type);
    //     std::cout << ", literal: " << token.literal << std::endl; 
        
    //     token = lexer.next_token();
    // }

    std:: cout << "Hello!" << std::endl;
}