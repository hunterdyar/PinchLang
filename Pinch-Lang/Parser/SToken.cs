using Superpower.Display;

namespace ShapesDeclare;

public enum SToken
{
	None,
	[Token(Category = "Identifier")] Identifier,
	[Token(Category = "Identifier")] DotIdentifier,
	[Token(Category = "Identifier")] UnderscoreIdentifier,
	[Token(Category = "Identifier")] AtIdentifier,
	[Token(Example = ":")] Colon,
	[Token(Example = ">")] ChevRight,
	[Token(Example = ".")] Dot,
	[Token(Example = "(")] LParen,
	[Token(Example = ")")] RParen,
	[Token(Example = "{")] LBracket,
	[Token(Example = "}")] RBracket,
	[Token(Example = "[")] LBrace,
	[Token(Example = "]")] RBrace,
	[Token(Example = "/")] Slash,
	[Token(Example = "*")] Asterisk,
	[Token(Example = "+")] Plus,
	[Token(Example = "-")] Minus,
	[Token(Example = "@")] At,
	[Token(Example = "_")] Underscore,
	[Token(Category = "Keyword")] Set,
	[Token(Category = "Keyword")] Group,
	[Token(Category = "Literal")] String,
	[Token(Category = "Literal")] Integer,
	[Token(Category = "Literal")] Double,
	[Token(Category = "Whitespace")] Newline,
}