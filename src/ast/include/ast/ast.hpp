#pragma once

#include <string>
#include <token/token.hpp>
#include <vector>

namespace ast {
    enum class NodeType {
        STATEMENT,
        EXPRESSION
    };

    class Node {
    public:
        virtual ~Node() = default;
        virtual const std::string tokenLiteral() const = 0;
        virtual const std::string string() const = 0;
    };

    class Program final : public Node {
    public:
        explicit Program(const std::vector<Node *> &s) : statements{s} {
        }
        const std::string tokenLiteral() const override;
        const std::string string() const override;
        constexpr NodeType type() const {
            return NodeType::EXPRESSION;
        }

    private:
        const std::vector<Node *> &statements;
    };

    class Identifier final : public Node {
    public:
        Identifier(const token::Token &t, const std::string &v) : token{t}, value{v} {
        }
        const std::string tokenLiteral() const override {
            return token.literal;
        }
        const std::string string() const override {
            return value;
        }
        constexpr NodeType type() const {
            return NodeType::EXPRESSION;
        }

    private:
        const token::Token token;
        const std::string &value;
    };

    class LetStatement final : public Node {
    public:
        LetStatement(const token::Token &t, const Identifier &n, const Node &v) : token{t}, name{n}, value{v} {
        }
        const std::string tokenLiteral() const override {
            return token.literal;
        }
        const std::string string() const override;
        constexpr NodeType type() const {
            return NodeType::STATEMENT;
        }

    private:
        const token::Token token;
        const Identifier &name;
        const Node &value;
    };

    class BlockStatement final : public Node {
    public:
        BlockStatement(const token::Token &t, const std::vector<Node *> &s) : token{t}, statements{s} {
        }
        const std::string tokenLiteral() const override {
            return token.literal;
        }
        const std::string string() const override;
        constexpr NodeType type() const {
            return NodeType::STATEMENT;
        }

    private:
        const token::Token token;
        const std::vector<Node *> &statements;
    };

    class ReturnStatement final : public Node {
    public:
        ReturnStatement(const token::Token &t, const Node &rv) : token{t}, returnValue{rv} {
        }
        const std::string tokenLiteral() const override {
            return token.literal;
        }
        const std::string string() const override;
        constexpr NodeType type() const {
            return NodeType::STATEMENT;
        }

    private:
        const token::Token token;
        const Node &returnValue;
    };

    class FunctionLiteral final : public Node {
    public:
        FunctionLiteral(const token::Token &t, const std::vector<Node *> &params, const BlockStatement &b) :
            token{t}, parameters{params}, body{b} {
        }
        const std::string tokenLiteral() const override {
            return token.literal;
        }
        const std::string string() const override;
        constexpr NodeType type() const {
            return NodeType::STATEMENT;
        }

    private:
        const token::Token token;
        const std::vector<Node *> &parameters;
        const BlockStatement &body;
    };

    class ExpressionStatement final : public Node {
    public:
        ExpressionStatement(const token::Token &t, const Node &e) : token{t}, expression{e} {
        }
        const std::string tokenLiteral() const override {
            return token.literal;
        }
        const std::string string() const override {
            return expression.string();
        }
        constexpr NodeType type() const {
            return NodeType::EXPRESSION;
        }

    private:
        const token::Token token;
        const Node &expression;
    };

    class PrefixExpression final : public Node {
    public:
        PrefixExpression(const token::Token &t, const std::string &op, const Node &r) :
            token{t}, prefixOperator{op}, rightExpression{r} {
        }
        const std::string tokenLiteral() const override {
            return token.literal;
        }
        const std::string string() const override;
        constexpr NodeType type() const {
            return NodeType::EXPRESSION;
        }

    private:
        const token::Token token;
        const std::string &prefixOperator;
        const Node &rightExpression;
    };

    class InfixExpression final : public Node {
    public:
        InfixExpression(const token::Token &t, const std::string &op, const Node &l, const Node &r) :
            token{t}, infixOperator{op}, left{l}, right{r} {
        }
        const std::string tokenLiteral() const override {
            return token.literal;
        }
        const std::string string() const override;
        constexpr NodeType type() const {
            return NodeType::EXPRESSION;
        }

    private:
        const token::Token token;
        const std::string &infixOperator;
        const Node &left;
        const Node &right;
    };

    class IntegerLiteral final : public Node {
    public:
        IntegerLiteral(const token::Token &t, const long v) : token{t}, value{v} {
        }
        const std::string tokenLiteral() const override {
            return token.literal;
        }
        const std::string string() const override {
            return token.literal;
        }
        constexpr NodeType type() const {
            return NodeType::EXPRESSION;
        }

    private:
        token::Token token;
        long value;
    };

    class Boolean final : public Node {
    public:
        Boolean(const token::Token &t, const bool v) : token{t}, value{v} {
        }
        const std::string tokenLiteral() const override {
            return token.literal;
        }
        const std::string string() const override {
            return token.literal;
        }
        constexpr NodeType type() const {
            return NodeType::EXPRESSION;
        }

    private:
        const token::Token token;
        const bool value;
    };
} // namespace ast
