using Superpower.Display;

namespace ShapesDeclare;

public enum SToken
{
	None,
	[Token(Category = "Identifier")] Identifier,
	[Token(Category = "Identifier")] DotIdentifier,
	[Token(Category = "Identifier")] UnderscoreIdentifier,
	[Token(Category = "Identifier")] AtIdentifier,
	[Token(Category = "Identifier")] BangIdentifier,
	[Token(Example = ":")] Colon,
	[Token(Example = "=")] Equals,
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

	[Token(Category = "Prefix", Example = "@")] At,
	[Token(Category = "Prefix", Example = "_")] Underscore,
	[Token(Category = "Prefix", Example = "#")] Octothorpe,
	[Token(Category = "Prefix", Example = "!")] Bang,

	// [Token(Category = "Prefix", Example = "$")] Dollar,
	[Token(Category = "Keyword")] Set,
	[Token(Category = "Keyword")] Group,
	[Token(Category = "Keyword")] Global,
	[Token(Category = "Literal")] String,
	[Token(Category = "Literal")] Integer,
	[Token(Category = "Literal")] Double,
	[Token(Category = "Whitespace")] Newline,
}