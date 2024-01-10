using JoeLang.AST;
using JoeLang.Constants;
using JoeLang.Object;

namespace JoeLang.Evaluator;

public class JoeEvaluator
{
    public IJoeObject? Evaluate(AST.INode node, JoeEnvironment environment)
    {
        switch (node)
        {
            // statements
            case AST.JoeProgram program:
                return EvaluateProgram(program, environment);
            case AST.ExpressionStatement expressionStatement:
                return Evaluate(expressionStatement.Expression, environment);
            case AST.BlockStatement blockStatement: 
                return EvaluateBlockStatement(blockStatement, environment);
            case AST.ReturnStatement returnStatement:
                var returnValue = Evaluate(returnStatement.ReturnValue, environment);
                if (IsError(returnValue))
                    return returnValue;
                return new JoeReturnValue(returnValue);
            case AST.LetStatement letStatement:
                var letValue = Evaluate(letStatement.Value, environment);
                if (IsError(letValue))
                    return letValue;
                environment.Set(((AST.LetStatement)node).Name.Value, letValue);
                break;

            // expressions
            case AST.IntegerLiteral integerLiteral:
                return new JoeInteger(integerLiteral.Value);
            case AST.Boolean booleanObject:
                return BoolToBooleanObject(booleanObject.Value);
            case AST.StringLiteral stringLiteral:
                return new JoeString(stringLiteral.Value);
            case AST.ArrayLiteral arrayLiteral:
                var arrayElements = EvaluateExpressions(arrayLiteral.Elements, environment);
                if (arrayElements.Length == 1 && IsError(arrayElements[0]))
                    return arrayElements[0];
                return new JoeArray(arrayElements);
            case AST.IndexExpression indexExpression:
                var left = Evaluate(indexExpression.Left, environment);
                if (IsError(left)) 
                    return left;
                var index = Evaluate(indexExpression.Index, environment);
                if (IsError(index))
                    return index;
                return EvaluateIndexExpression(left, index);
            case AST.FunctionLiteral functionLiteral: 
                var parameters = functionLiteral.Parameters;
                var body = functionLiteral.Body;
                return new JoeFunction(parameters, body, environment);
            case AST.CallExpression callExpression: 
                var function = Evaluate(callExpression.Function, environment);
                if (IsError(function))
                    return function;
                var arguments = EvaluateExpressions(callExpression.Arguments, environment);
                if (arguments.Length == 1 && IsError(arguments[0]))
                    return arguments[0];

                return ApplyFunction(function, arguments);
            case AST.PrefixExpression prefixExpression: 
                var prefixRight = Evaluate(prefixExpression.Right, environment);
                if (IsError(prefixRight)) 
                    return prefixRight;
                return EvaluatePrefixExpression(prefixExpression.Operator, prefixRight);
            case AST.InfixExpression infixExpression:
                var infixLeft = Evaluate(infixExpression.Left, environment);
                if (IsError(infixLeft)) 
                    return infixLeft;
                var infixRight = Evaluate(infixExpression.Right, environment);
                if (IsError((infixRight))) 
                    return infixRight;
                return EvaluateInfixExpression(infixExpression.Operator, infixLeft, infixRight);
            case AST.IfExpression ifExpression:
                return EvaluateIfExpression(ifExpression, environment);
            case AST.Identifier identifier:
                return EvaluateIdentifier(identifier, environment);
            case AST.HashLiteral hashLiteral:
                return EvaluateHashLiteral(hashLiteral, environment);
        }

        return null;
    }

    private IJoeObject? EvaluateHashLiteral(HashLiteral hashLiteral, JoeEnvironment environment)
    {
        var pairs = new Dictionary<HashKey, HashPair>();

        foreach (var keyValuePair in hashLiteral.Pairs) 
        { 
            var key = Evaluate(keyValuePair.Key, environment);
            if (IsError(key))
                return key;

            var value = Evaluate(keyValuePair.Value, environment);
            if (IsError(value))
                return value;

            // need to convert to explict joestring, joebool, or other hashable object
            // I should figure out how to do this check better
            //if (typeof(IHashable).IsAssignableFrom(key.GetType()))
            //    return new JoeError($"unusable as hash key: {key.Type()}");
            HashKey? hashKey = null;
            if (key is JoeString joeString)
                hashKey = joeString.HashKey();
            else if (key is JoeBoolean joeBoolean)
                hashKey = joeBoolean.HashKey();
            else if (key is JoeInteger joeInteger)
                hashKey = joeInteger.HashKey();
            
            if (hashKey == null)
                return new JoeError($"unusable as hash key: {key.Type()}");
            
            pairs[hashKey.GetValueOrDefault()] = new HashPair(key, value);
        }

        return new JoeHash(pairs);
    }

    private IJoeObject EvaluateHashIndexExpression(IJoeObject hash, IJoeObject index)
    {
        var hashObject = (JoeHash)hash;

        HashKey? hashKey = null;
        if (index is JoeString joeString)
            hashKey = joeString.HashKey();
        else if (index is JoeBoolean joeBoolean)
            hashKey = joeBoolean.HashKey();
        else if (index is JoeInteger joeInteger)
            hashKey = joeInteger.HashKey();

        if (hashKey == null)
            return new JoeError($"unusable as hash key: {index.Type()}");

        if (hashObject.Pairs.TryGetValue(hashKey.GetValueOrDefault(), out HashPair hashPair))
            return hashPair.value;
        return EvaluatorConstants.NULL;
    }

    private IJoeObject EvaluateIndexExpression(IJoeObject left, IJoeObject index)
    {
        if (left is JoeArray array && index is JoeInteger integer)
            return EvaluateArrayIndexExpression(array, integer);
        else if (left is JoeHash)
            return EvaluateHashIndexExpression(left, index);
        return new JoeError($"index operator not supported: {left.Type()}");
    }

    private IJoeObject EvaluateArrayIndexExpression(JoeArray array, JoeInteger integer)
    {
        var i = integer.Value;
        var max = array.Elements.Length;

        return i >= 0 && i < max ? array.Elements[i] : EvaluatorConstants.NULL;
    }

    private IJoeObject? EvaluateProgram(AST.JoeProgram program, JoeEnvironment environment)
    {
        IJoeObject? result = null;

        foreach(var statement in program.Statements) 
        { 
            result = Evaluate(statement, environment);

            switch (result)
            {
                case JoeReturnValue returnValue:
                    return returnValue.Value;
                case JoeError error:
                    return error;
            }
        }

        return result;
    }

    private IJoeObject? EvaluateBlockStatement(AST.BlockStatement block, JoeEnvironment environment)
    {
        IJoeObject? result = null;

        foreach (var statement in block.Statements) 
        { 
            result = Evaluate(statement, environment);

            if (result != null && (result is JoeReturnValue ||result is JoeError))
                return result;
        }

        return result;
    }

    private IJoeObject EvaluateInfixExpression(string op, IJoeObject left, IJoeObject right) 
    { 
        if (left is JoeInteger leftInt && right is JoeInteger rightInt)
            return EvaluateIntegerInfixExpression(op, leftInt, rightInt);

        if (left is JoeString leftString && right is JoeString rightString)
            return EvaluateStringInfixExpression(op, leftString, rightString);
        
        if (left.Type() != right.Type())
            return new JoeError($"type mismatch: {left.Type()} {op} {right.Type()}");

        return op switch
        {
            "==" => BoolToBooleanObject(left == right),
            "!=" => BoolToBooleanObject(left != right),
            _    => new JoeError($"unknown operator: {left.Type()} {op} {right.Type()}"),
        };
    }

    private IJoeObject EvaluateIntegerInfixExpression(string op, JoeInteger left, JoeInteger right)
    {
        return op switch
        {
            "+"  => new JoeInteger(left.Value + right.Value),
            "-"  => new JoeInteger(left.Value - right.Value),
            "*"  => new JoeInteger(left.Value * right.Value),
            "/"  => new JoeInteger(left.Value / right.Value),
            "<"  => BoolToBooleanObject(left.Value < right.Value),
            ">"  => BoolToBooleanObject(left.Value > right.Value),
            "==" => BoolToBooleanObject(left.Value == right.Value),
            "!=" => BoolToBooleanObject(left.Value != right.Value),
            _    => new JoeError($"unknown operator: {left.Type()} {op} {right.Type()}"),
        };
    }

    private IJoeObject EvaluateStringInfixExpression(string op, JoeString left, JoeString right) 
    { 
        if (op != "+")
            return new JoeError($"unknown operator: {left.Type()} {op} {right.Type()}");

        return new JoeString(left.Value + right.Value);
    }

    private IJoeObject EvaluatePrefixExpression(string op, IJoeObject right)
    {
        return op switch
        {
            "!" => EvaluateBangOperatorExpression(right),
            "-" => EvaluateMinusPrefixOperatorExpression(right),
            _   => new JoeError($"unknown operator: {op}{right.Type()}")
        };
    }

    private IJoeObject EvaluateMinusPrefixOperatorExpression(IJoeObject right)
    {
        return right is JoeInteger integer ? new JoeInteger(-integer.Value) :
            new JoeError($"unknown operator: -{right.Type()}");
    }

    private IJoeObject EvaluateBangOperatorExpression(IJoeObject right)
    {
        return right switch
        {
            JoeBoolean => right == EvaluatorConstants.TRUE ? 
                EvaluatorConstants.FALSE : EvaluatorConstants.TRUE,
            JoeNull    => EvaluatorConstants.TRUE,
            _          => EvaluatorConstants.FALSE

        };
    }

    private IJoeObject[] EvaluateExpressions(AST.IExpressionNode[] expressions, 
        JoeEnvironment environment)
    {
        var result = new List<IJoeObject>();

        foreach (var expression in expressions) 
        { 
            var evaluated = Evaluate(expression, environment);
            if (IsError(evaluated))
                return new IJoeObject[] { evaluated };
            result.Add(evaluated);
        }

        return result.ToArray();
    }

    private IJoeObject? EvaluateIfExpression(AST.IfExpression ifExpression, 
        JoeEnvironment environment)
    {
        var condition = Evaluate(ifExpression.Condition, environment);
        if (IsError(condition)) 
            return condition;

        if (IsTruthy(condition))
            return Evaluate(ifExpression.Consequence, environment);
        else if (ifExpression.Alternative != null)
            return Evaluate(ifExpression.Alternative, environment);
        return EvaluatorConstants.NULL;
    }

    private IJoeObject? EvaluateIdentifier(AST.Identifier node, JoeEnvironment environment)
    {
        if (Builtins.builtins.TryGetValue(node.Value, out JoeBuiltin? builtin))
            return builtin;

        return environment.Get(node.Value) ?? new JoeError($"identifier not found: {node.Value}");
    }

    private IJoeObject ApplyFunction(IJoeObject function, IJoeObject[] args)
    {
        switch (function)
        {
            case JoeFunction fn:
                var extendedEnvironment = ExtendedFunctionEnvironment(fn, args);
                var evaluated = Evaluate(fn.Body, extendedEnvironment);
                return UnwrapReturnValue(evaluated);
            case JoeBuiltin fn:
                return fn.Function(args);
            default:
                return new JoeError($"not a function: {function.Type()}");
        }
    }

    private JoeEnvironment ExtendedFunctionEnvironment(JoeFunction function, IJoeObject[] args) 
    { 
        var environment = new JoeEnvironment(function.Environment);

        for (int i = 0; i < function.Parameters.Length; i++)
            environment.Set(function.Parameters[i].Value, args[i]);

        return environment;
    }

    private IJoeObject UnwrapReturnValue(IJoeObject obj)
    {
        if (obj is JoeReturnValue returnValue)
            return returnValue.Value;

        return obj;
    }

    private static bool IsError(IJoeObject? obj)
    {
        return obj != null && obj.Type() == ObjectConstants.ERROR_OBJECT;
    }

    private static bool IsTruthy(IJoeObject? obj)
    {
        return obj switch
        {
            JoeNull => false,
            JoeBoolean => ((JoeBoolean)obj).Value,
            _ => true,
        };
    }

    private static JoeBoolean BoolToBooleanObject(bool input) 
    { 
        return input ? EvaluatorConstants.TRUE : EvaluatorConstants.FALSE;
    }
}
