using JoeLang.AST;
using JoeLang.Evaluator;
using JoeLang.Lexer;
using JoeLang.Object;
using JoeLang.Parser;
using JoeLang.REPL;

namespace JoeLang;

class JoeLang
{
    public static void Main(string[] args)
    {
        if (args.Length == 0) {
            var userName = Environment.UserName;

            Console.WriteLine($"Hello {userName}! This is the Joe programming language.");
            Console.WriteLine("Please type in commands.");

            JoeREPL.Start();
            return;
        }

        EvaluateFile(args[0]);
    }

    private static void EvaluateFile(string filename) 
    {
        string input = ReadFile(filename);
        JoeEnvironment environment = new();
        JoeEvaluator evaluator = new();

        JoeLexer lexer = new(input);
        DescentParser parser = new(lexer);
        JoeProgram program = parser.ParseProgram();

        if (parser.Errors.Length > 0)
        {
            Console.WriteLine("Encountered parser errors:");
            foreach (var error in parser.Errors)
                Console.WriteLine('\t' + error + '\n');
        }

        var evaluated = evaluator.Evaluate(program,environment);
    }

    private static string ReadFile(string filename) 
    {
        StreamReader sr = new(filename);
        return sr.ReadToEnd();
    }
}
