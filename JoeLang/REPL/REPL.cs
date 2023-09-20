using JoeLang.Lexer;
using JoeLang.Token;

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
        while (true) 
        {
            Console.Write(PROMPT);
            var line = Console.ReadLine();
            if (line == null)
                return;

            var lexer = new JoeLexer(line);

            for (var token = lexer.NextToken(); token.Type != Tokens.EOF; token = lexer.NextToken()) 
            { 
                Console.WriteLine("Type: {0}, Literal: {1}", token.Type, token.Literal);
            }
        }
    }
}