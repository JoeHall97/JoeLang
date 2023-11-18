using JoeLang.Constants;
using JoeLang.Evaluator;
using JoeLang.Lexer;
using JoeLang.Object;
using JoeLang.Parser;

namespace JoeLang.REPL;

public static class JoeREPL
{
    private const string PROMPT = ">> ";
    // coffee art from: https://www.asciiart.eu/food-and-drinks/coffee-and-tea
    // word art from: https://patorjk.com/software/taag/#p=display&f=Graffiti&t=Joe
    private const string COFFEE =
@"
    (  )   (   )  )
     ) (   )  (  (
     ( )  (    ) )
     _____________
    <_____________> ___
    |             |/ _ \
    |               | | |
    |               |_| |
 ___|             |\___/
/    \___________/    \
\_____________________/

     ____.              
    |    | ____   ____  
    |    |/  _ \_/ __ \ 
/\__|    (  <_> )  ___/ 
\________|\____/ \___  >
                     \/ 
";

    public static void Start()
    {
        Console.WriteLine(COFFEE);
        var environment = new JoeEnvironment();
        var evaluator = new JoeEvaluator();

        while (true) 
        {
            Console.Write(PROMPT);
            var line = Console.ReadLine();
            if (line == null)
                return;

            var lexer = new JoeLexer(line);
            var parser = new DescentParser(lexer);
            var program = parser.ParseProgram();

            if (parser.Errors.Length > 0)
            {
                PrintParserErrors(parser.Errors);
                continue;
            }

            var evaluated = evaluator.Evaluate(program,environment);
            if (evaluated != null)
                Console.WriteLine(evaluated.Inspect());
        }
    }

    private static void PrintParserErrors(string[] errors) 
    {
        Console.WriteLine("Encountered parser errors:");
        foreach (var error in errors) 
            Console.WriteLine('\t' + error + '\n');
    }
}