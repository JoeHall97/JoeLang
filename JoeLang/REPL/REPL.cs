using JoeLang.Lexer;
using JoeLang.Token;

namespace JoeLang.REPL;

public static class JoeREPL
{
    private const string PROMPT = ">> ";

    public static void start()
    {
        while (true) 
        {
            Console.Write(PROMPT);
            var line = Console.ReadLine();
            if (line == null)
                return;

            var lexer = new JoeLexer(line);

            for (var token = lexer.nextToken(); token.Type != Tokens.EOF; token = lexer.nextToken()) 
            { 
                Console.WriteLine("Type: {0}, Literal: {1}", token.Type, token.Literal);
            }
        }
    }
}