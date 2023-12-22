#include <sstream>
#include "ast.hpp"

Program::Program(std::vector<Node> s) {
    statements = s;
}

std::string Program::token_literal() {
    if (statements.size() > 0)
        return statements[0].token_literal();
    return "";
}

std::string Program::string() {
    std::stringstream sstream;

    for (auto s : statements) {
        sstream << s.string();
    }

    return sstream.str();
}

Identifier::Identifier(Token t, std::string v) {
    token = t;
    value = v;
}

std::string Identifier::token_literal() {
    return token.literal;
}

std::string Identifier::string() {
    return value;
}

LetStatement::LetStatement(Token t, Identifier* n, Node v) {
    token = t;
    name = n;
    value = v;
}

std::string LetStatement::token_literal() {
    return token.literal;
}

std::string LetStatement::string() {
    std::stringstream sstream;

    sstream << token.literal << " ";
    sstream << name->string() << " = ";
    sstream << value.string();
    sstream << ";";

    return sstream.str();
}

BlockStatement::BlockStatement(Token t, std::vector<Node> s) {
    token = t;
    statements = s;
}

std::string BlockStatement::token_literal() {
    return token.literal;
}

std::string BlockStatement::string() {
    std::stringstream sstream;

    for (auto s : statements) {
        sstream << s.string();
    }

    return sstream.str();
}

ReturnStatment::ReturnStatment(Token t, Node rv) {
    token = t;
    returnValue = rv;
}

std::string ReturnStatment::token_literal() {
    return token.literal;
}

std::string ReturnStatment::string() {
    std::stringstream sstream;

    sstream << token.literal << ' ' << returnValue.string() << ';';

    return sstream.str();
}

FunctionLiteral::FunctionLiteral(Token t, std::vector<Node>* p, BlockStatement* b) {
    token = t;
    parameters = p;
    body = b;
}

std::string FunctionLiteral::token_literal() {
    return token.literal;
}

std::string FunctionLiteral::string() {
    std::stringstream sstream;

    sstream << token.literal;
    sstream << '(';
    for (int i = 0; i < parameters->size()-1; i++) {
        sstream << (*parameters)[i].string() << ", ";
    }
    sstream << (*parameters)[parameters->size()-1].string() << ')';
    sstream << body->string();

    return sstream.str();
}

ExpressionStatement::ExpressionStatement(Token t, Node e) {
    token = t;
    expression = e;
}

std::string ExpressionStatement::token_literal() {
    return token.literal;
}

std::string ExpressionStatement::string() {
    return expression.string();
}

PrefixExpression::PrefixExpression(Token t, std::string op, Node r) {
    token = t;
    prefixOperator = op;
    rightExpression = r;
}

std::string PrefixExpression::token_literal() {
    return token.literal;
}

std::string PrefixExpression::string() {
    std::stringstream sstream;

    sstream << '(';
    sstream << prefixOperator << rightExpression.string();
    sstream << ')';

    return sstream.str();
}

InfixExpression::InfixExpression(Token t, std::string op, Node l, Node r) {
    token = t;
    infixOperator = op;
    left = l;
    right = r;
}

std::string InfixExpression::token_literal() {
    return token.literal;
}

std::string InfixExpression::string() {
    std::stringstream sstream;

    sstream << '(';
    sstream << left.string() << ' ' << infixOperator << ' ' << right.string();
    sstream << ')';

    return sstream.str(); 
}

IntegerLiteral::IntegerLiteral(Token t, long v) {
    token = t;
    value = v;
}

std::string IntegerLiteral::token_literal() {
    return token.literal;
}

std::string IntegerLiteral::string() {
    return token.literal;
}

Boolean::Boolean(Token t, bool v) {
    token = t;
    value = v;
}

std::string Boolean::token_literal() {
    return token.literal;
}

std::string Boolean::string() {
    return token.literal;
}