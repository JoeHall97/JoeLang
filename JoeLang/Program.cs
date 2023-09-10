// See https://aka.ms/new-console-template for more information
using JoeLang.REPL;

namespace JoeLang;

class JoeLang
{
    public static void Main(string[] args)
    {
        var userName = Environment.UserName;

        Console.WriteLine($"Hello {userName}! This is the Joe programming language.");
        Console.WriteLine("Please type in commands.");

        JoeREPL.Start();
    }
}
