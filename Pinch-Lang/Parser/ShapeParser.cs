using System.Diagnostics.CodeAnalysis;
using ShapesDeclare.AST;
using Superpower;
using Superpower.Parsers;
using Expression = ShapesDeclare.AST.Expression;
using Identifier = ShapesDeclare.AST.Identifier;

namespace ShapesDeclare;

public static class ShapeParser
{
	// public static readonly TokenListParser<SToken, Expression> Expression =
	// 	from e in Identifier
	// 	//.Or
	// 	select e;
	
	//Expressions
	
	//identifiers are normal or "_" for when we are discarding the identifer.p

	private static TokenListParser<SToken, Expression> NumberLiteral { get; } =
		from num in Token.EqualTo(SToken.Integer).Or(Token.EqualTo(SToken.Double))
		select (Expression)new NumberLiteral(num.Span);

	private static TokenListParser<SToken, Expression> StringLiteral { get; } =
		from str in Token.EqualTo(SToken.String)
		select (Expression)new StringLiteral(str.Span);

	private static TokenListParser<SToken, Expression> Literal { get; } =
		from literal in
			NumberLiteral
			.Or(StringLiteral)
		select literal;
	static TokenListParser<SToken, Expression> NormalIdentifier { get; } =
		from id in Token.EqualTo(SToken.Identifier)
		select (Expression)new Identifier(id.Span);

	static TokenListParser<SToken, Expression> ThrowawayIdentifier { get; } =
		from id in Token.EqualTo(SToken.Underscore)
		// from expr in Token.EqualTo(SToken.Identifier)
		select (Expression)new ThrowawayIdentifier(id.Span);
	
	static TokenListParser<SToken, Expression> Identifier { get; } =
		from e in
			ThrowawayIdentifier
			.Or(NormalIdentifier)
		select e;
	
	static TokenListParser<SToken, Expression> IDTuple { get; }=
		from a in Identifier
		from _ in Token.EqualTo(SToken.Colon)
		from b in Identifier
		select (Expression)new IDTuple((Identifier)a, (Identifier)b);

	static TokenListParser<SToken, Expression> PrefixUnaryOperation { get; } =
		from op in Token.EqualTo(SToken.Plus).Or(Token.EqualTo(SToken.Minus))
		from exp in Expression
		select (Expression)new UnaryOperator(op, exp);
	static TokenListParser<SToken, Expression> Operation { get; } =
		from puo in PrefixUnaryOperation
		select (Expression)puo;
	static TokenListParser<SToken, Expression> Expression { get; }=
		from x in (TokenListParser<SToken, Expression>)
			Identifier
			.Or(Literal)
			.Or<SToken, Expression>(IDTuple)
			.Or(Operation)
			select x;
	
	//Statements
	static TokenListParser<SToken, Header> Header { get; }= 
		from _lb in Token.EqualTo(SToken.LBrace)
		from id in Identifier
		from _rb in Token.EqualTo(SToken.RBrace)
		select new Header(((Identifier)id).Value);
	
	static TokenListParser<SToken, Statement> Declaration { get; }=
		from ide in IDTuple
		from exprs in Expression.Many()
		select (Statement)new ShapeDeclaration((IDTuple)ide, exprs);
	
	static TokenListParser<SToken, Statement> FunctionCall { get; }=
		from id in Identifier
		from exprs in Expression.Many()
		select (Statement)new FunctionCall((Identifier) id, exprs);

	//'pushable' right now is just declarations i guess. groups and stuff later?
	private static TokenListParser<SToken, Statement> Push { get; } =
		from sb in Declaration
		from _ in Token.EqualTo(SToken.ChevRight)
		select (Statement)new Push((ShapeDeclaration)sb);
	
	static TokenListParser<SToken, Statement> Pop { get; } =
		from _ in Token.EqualTo(SToken.Dot)
		select (Statement)new Pop();
	
	static TokenListParser<SToken, Statement> Statement { get; }=
		from s in Push.Try()
			.Or(Pop)
			.Or(FunctionCall)
			.Or(Declaration) //has to be after Push, since they both look for Dec first.
		select s;

	static TokenListParser<SToken, Section> Section { get; }=
		from h in Header
		from stmnts in Statement.Many()
		select new Section(h, stmnts);

	static TokenListParser<SToken, Root> Root { get; } =
		from sections in Section.Many()
		select new Root(sections);

	private static TokenListParser<SToken, Root> Program { get; } = Root.AtEnd().Named("Program");
	
	public static bool TryParse(string input, [MaybeNullWhen(false)] out Root root, [MaybeNullWhen(true)]out string error)
	{
		var tokr = ShapeTokenizer.Tokenizer.TryTokenize(input);

		if (!tokr.HasValue)
		{
			root = AST.Root.Empty;
			error = tokr.ToString();
			return false;
		}
		
		var res = Program.TryParse(tokr.Value);
		if (res.HasValue)
		{
			root = res.Value;
			error = "";
			return true;
		}
		else
		{
			root = AST.Root.Empty;
			error = res.ToString();
			Console.WriteLine(error);
			return false;
		}
	}
}