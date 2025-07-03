#include <ast/ast.hpp>
#include <sstream>

const std::string ast::Program::tokenLiteral() const {
    if (statements.size() > 0)
        return statements[0]->tokenLiteral();
    return "";
}

const std::string ast::Program::string() const {
    std::stringstream sstream;

    for (auto s: statements) {
        sstream << s->string();
    }

    return sstream.str();
}

const std::string ast::LetStatement::string() const {
    std::stringstream sstream;

    sstream << token.literal << " ";
    sstream << name.string() << " = ";
    sstream << value.string();
    sstream << ";";

    return sstream.str();
}

const std::string ast::BlockStatement::string() const {
    std::stringstream sstream;

    for (auto s: statements) {
        sstream << s->string();
    }

    return sstream.str();
}

const std::string ast::ReturnStatement::string() const {
    std::stringstream sstream;

    sstream << token.literal << ' ' << returnValue.string() << ';';

    return sstream.str();
}

const std::string ast::FunctionLiteral::string() const {
    std::stringstream sstream;

    sstream << token.literal;
    sstream << '(';
    for (int i = 0; i < parameters.size() - 1; i++) {
        sstream << parameters[i]->string() << ", ";
    }
    sstream << parameters[parameters.size() - 1]->string() << ')';
    sstream << body.string();

    return sstream.str();
}

const std::string ast::PrefixExpression::string() const {
    std::stringstream sstream;

    sstream << '(';
    sstream << prefixOperator << rightExpression.string();
    sstream << ')';

    return sstream.str();
}

const std::string ast::InfixExpression::string() const {
    std::stringstream sstream;

    sstream << '(';
    sstream << left.string() << ' ' << infixOperator << ' ' << right.string();
    sstream << ')';

    return sstream.str();
}
