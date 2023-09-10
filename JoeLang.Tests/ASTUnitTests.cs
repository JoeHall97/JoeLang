using JoeLang.AST;
using JoeLang.Token;

namespace JoeLang.Tests;

public class ASTUnitTests
{
    [Fact]
    public void TestToString()
    {
        var nameIdentifier = new Identifier(new JoeToken(Tokens.IDENT, "myVar"), "myVar");
        var valueIdentifier = new Identifier(new JoeToken(Tokens.IDENT, "anotherVar"), "anotherVar");

        var letStatement = new LetStatement(new JoeToken(Tokens.VAR, "var"), nameIdentifier, valueIdentifier);

        var program = new JoeProgram(new IStatementNode[] { letStatement });

        Assert.Equal("var myVar = anotherVar;", program.ToString());
    }
}
