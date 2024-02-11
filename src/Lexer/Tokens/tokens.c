#include "tokens.h"
#include <stdbool.h>

static struct IdentTable {
	char* s;
	TokenType type;
} ident_table[] = {
	{"fn", FUNCTION},
	{"let", LET},
	{"false", FALSE},
	{"true", TRUE},
	{"if", IF},
	{"else", ELSE},
	{"return", RETURN},
	{"macro", MACRO}
};

#define LOOK_UP_TABLE_SIZE (sizeof(ident_table) / sizeof(struct IdentTable))

TokenType lookupident(char* s, int len) {
	for (int i = 0; i < LOOK_UP_TABLE_SIZE; i++) {
		for (int j = 0; j < len; j++) {
			if (ident_table[i].s[j] == '\0')
				return ident_table[i].type;
			if (s[j] != ident_table[i].s[j])
				break;
		}
	}

	return IDENT;
}
