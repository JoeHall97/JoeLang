#ifndef AST_H
#define AST_H
#include <vector>
#include <string>
#include "../tokens/token.hpp"

class Node {
    public:
        virtual std::string token_literal() = 0;
        virtual std::string string() = 0;
};

enum NodeType {
    STATEMENT,
    EXPRESSION
};

class Program : public Node {
    public:
        Program (std::vector<Node> s);
        std::string token_literal();
        std::string string();
    private:
        std::vector<Node> statements;
};

class Identifier : public Node {
    public:
        Identifier (Token t, std::string v);
        std::string token_literal();
        std::string string();
        const NodeType type = EXPRESSION;
    private:
        Token token;
        std::string value;
};

class LetStatement : public Node {
    public:
        LetStatement (Token t, Identifier* n, Node v);
        std::string token_literal();
        std::string string();
        const NodeType type = STATEMENT;
    private:
        Token token;
        Identifier* name;
        Node value;
};

class BlockStatement : public Node {
    public:
        BlockStatement (Token t, std::vector<Node> s);
        std::string token_literal();
        std::string string();
        const NodeType type = STATEMENT;
    private:
        Token token;
        std::vector<Node> statements;
};

// class IfExpression : public Node {
//     public:
//         IfExpression (Token t, Node c, )
// }

class ReturnStatment : public Node {
    public:
        ReturnStatment (Token t, Node rv);
        std::string token_literal();
        std::string string();
        const NodeType type = STATEMENT;
    private:
        Token token;
        Node returnValue;
};

class FunctionLiteral : public Node {
    public:
        FunctionLiteral (Token t, std::vector<Node>* p, BlockStatement* b);
        std::string token_literal();
        std::string string();
        const NodeType type = STATEMENT;
    private:
        Token token;
        std::vector<Node>* parameters;
        BlockStatement* body;
};

class ExpressionStatement : public Node {
    public:
        ExpressionStatement (Token t, Node e);
        std::string token_literal();
        std::string string();
        const NodeType type = EXPRESSION;
    private:
        Token token;
        Node expression;
};

class PrefixExpression : public Node {
    public:
        PrefixExpression (Token t, std::string op, Node r);
        std::string token_literal();
        std::string string();
        const NodeType type = EXPRESSION;
    private:
        Token token;
        std::string prefixOperator;
        Node rightExpression;
};

class InfixExpression : public Node {
    public:
        InfixExpression (Token t, std::string op, Node l, Node r);
        std::string token_literal();
        std::string string();
        const NodeType type = EXPRESSION;
    private:
        Token token;
        std::string infixOperator;
        Node left;
        Node right;
};

class IntegerLiteral : public Node {
    public:
        IntegerLiteral (Token t, long v);
        std::string token_literal();
        std::string string();
        const NodeType type = EXPRESSION;
    private:
        Token token;
        long value;
};

class Boolean : public Node {
    public:
        Boolean (Token t, bool v);
        std::string token_literal();
        std::string string();
        const NodeType type = EXPRESSION;
    private:
        Token token;
        bool value;
};



#endif