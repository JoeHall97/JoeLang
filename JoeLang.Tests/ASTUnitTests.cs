using JoeLang.AST;
using JoeLang.Token;
using JoeLang.Constants;

namespace JoeLang.Tests;

public class ASTUnitTests
{
    [Fact]
    public void TestToString()
    {
        var nameIdentifier = new Identifier(
            new JoeToken(TokenConstants.Ident, "myVar"), 
            "myVar"
        );
        var valueIdentifier = new Identifier(
            new JoeToken(TokenConstants.Ident, "anotherVar"), 
            "anotherVar"
        );

        var letStatement = new LetStatement(
            new JoeToken(TokenConstants.Let, "let"), 
            nameIdentifier, 
            valueIdentifier
        );

        var program = new JoeProgram(new IStatementNode[] { letStatement });

        Assert.Equal("let myVar = anotherVar;", program.ToString());
    }
}
