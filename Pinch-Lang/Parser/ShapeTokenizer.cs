using ShapesDeclare.AST;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using Superpower.Tokenizers;
using Identifier = Superpower.Parsers.Identifier;

namespace ShapesDeclare;

public static class ShapeTokenizer
{
	public static Tokenizer<SToken> Tokenizer = new TokenizerBuilder<SToken>()
		//COMMENTS
		//have to put newline before whitespace ignoring.
		.Match(Span.EqualTo(Environment.NewLine), SToken.Newline)
		.Match(Span.EqualTo("\r\n"), SToken.Newline)
		.Match(Span.EqualTo("\n"), SToken.Newline)

		.Ignore(Span.WhiteSpace)//hmmmmmm is this breaking newline?
	//	.Ignore(Character.In([' ', '\t']))
		.Ignore(Span.WhiteSpace)
		.Ignore(Comment.CPlusPlusStyle)
		.Ignore(Comment.CStyle)
		.Ignore(Comment.ShellStyle)
		

		//OPERATORS and SUGAR
		.Match(Character.EqualTo(':'), SToken.Colon)
		.Match(Character.EqualTo('>'), SToken.ChevRight)
		.Match(Character.EqualTo('.'), SToken.Dot)
		.Match(Character.EqualTo('('), SToken.LParen)
		.Match(Character.EqualTo(')'), SToken.RParen)
		.Match(Character.EqualTo('['), SToken.LBrace)
		.Match(Character.EqualTo(']'), SToken.RBrace)
		.Match(Character.EqualTo('+'), SToken.Plus)
		.Match(Character.EqualTo('-'), SToken.Minus)
		.Match(Character.EqualTo('*'), SToken.Asterisk)
		.Match(Character.EqualTo('/'), SToken.Slash)
		.Match(Character.EqualTo('@'), SToken.At)
		.Match(Character.EqualTo('_'), SToken.Underscore)
		
		//literals
		//strings and integers
		.Match(QuotedString.CStyle, SToken.String)
		.Match(Numerics.Integer, SToken.Integer)
		.Match(Numerics.DecimalDouble, SToken.Double)
		
		//Identifier
		.Match(Identifier.CStyle, SToken.Identifier)
		
		//KEYWORDS
		.Match(Span.EqualTo("set"), SToken.Set)
		.Match(Span.EqualTo("group"), SToken.Group)

		
		.Build();

	
	//lazy wrapper for cstyle; but this just gets replaced with our own later
	public static Result<SToken> SIdentifier(TextSpan sp)
	{
		var r = Identifier.CStyle.Invoke(sp);
		if (!r.HasValue)
		{
			return Result.Empty<SToken>(sp);
		}

		return Result.Value(SToken.Identifier, r.Location, r.Remainder);
	}

	// public static Result<SToken> SWhitespace(TextSpan sp)
	// {
	// 	
	// 	if (!r.HasValue)
	// 	{
	// 		return Result.Empty<SToken>(sp);
	// 	}
	//
	// 	return Result.Value(SToken.Identifier, r.Location, r.Remainder);
	// }
}