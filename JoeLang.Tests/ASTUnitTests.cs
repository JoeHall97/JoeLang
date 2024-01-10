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
            new JoeToken(TokenConstants.IDENT, "myVar"), 
            "myVar"
        );
        var valueIdentifier = new Identifier(
            new JoeToken(TokenConstants.IDENT, "anotherVar"), 
            "anotherVar"
        );

        var letStatement = new LetStatement(
            new JoeToken(TokenConstants.LET, "let"), 
            nameIdentifier, 
            valueIdentifier
        );

        var program = new JoeProgram(new IStatementNode[] { letStatement });

        Assert.Equal("let myVar = anotherVar;", program.ToString());
    }
}
