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
	[Token(Category = "Operator", Example = "/")] Slash,
	[Token(Category = "Operator", Example = "*")] Asterisk,
	[Token(Category = "Operator", Example = "+")] Plus,
	[Token(Category = "Operator", Example = "-")] Minus,
	[Token(Category = "Operator", Example = "%")] Percentage,
	[Token(Example = "@")] At,
	[Token(Example = "_")] Underscore,
	[Token(Category = "Keyword")] Set,
	[Token(Category = "Keyword")] Group,
	[Token(Category = "Literal")] String,
	[Token(Category = "Literal")] Integer,
	[Token(Category = "Literal")] Double,
	[Token(Category = "Whitespace")] Newline,
}