#include <stdio.h>
#include "JoeLang.h"

int main()
{
	char input[] = "var a = 5;";
	Lexer l = CreateLexer(input, 10);
	return 0;
}
