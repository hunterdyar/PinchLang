using System.Diagnostics.CodeAnalysis;
using ShapesDeclare.AST;
using Superpower;
using Superpower.Model;
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
	
	static TokenListParser<SToken, Expression> NormalIdentifier { get; } =
		from id in Token.EqualTo(SToken.Identifier)
		select (Expression)new Identifier(id.Span);

	static TokenListParser<SToken, Expression> DotIdentifier { get; } =
		from id in Token.EqualTo(SToken.DotIdentifier)
		select (Expression)new Identifier(id.Span, IdentPrefix.Dot);

	static TokenListParser<SToken, Expression> AtIdentifier { get; } =
		from id in Token.EqualTo(SToken.AtIdentifier)
		select (Expression)new Identifier(id.Span, IdentPrefix.At);

	static TokenListParser<SToken, Expression> UnderscoreIdentifier { get; } =
		from id in Token.EqualTo(SToken.UnderscoreIdentifier)
		select (Expression)new Identifier(id.Span, IdentPrefix.Underscore);

	static TokenListParser<SToken, Expression> ThrowawayIdentifier { get; } =
		from id in Token.EqualTo(SToken.Underscore)
		// from expr in Token.EqualTo(SToken.Identifier)
		select (Expression)new ThrowawayIdentifier(id.Span);

	public static TokenListParser<SToken, Expression> Identifier { get; } =
		from e in
			ThrowawayIdentifier
			.Or(NormalIdentifier)
			.Or(DotIdentifier)
			.Or(UnderscoreIdentifier)
			.Or(AtIdentifier)
		select e;
	
	public static TokenListParser<SToken, Expression> ExpressionIdentifier { get; } =
		from id in NormalIdentifier
			.Or(UnderscoreIdentifier)
		select id;
	
	static TokenListParser<SToken, Expression> KeyValueTuple { get; }=
		from a in Identifier.Try()
		from _ in Token.EqualTo(SToken.Colon).Try()
		from b in Expression
		select (Expression)new KeyValueTuple((Identifier)a, (Expression)b);
	
	public static TokenListParser<SToken, Expression> Expression { get; }=
		from x in (TokenListParser<SToken, Expression>)
			ExprParser.Expr
			.Or(ExpressionIdentifier) //the subset of identifiers that can be used as values (_ or no prefix)
			.Or(KeyValueTuple)
			//we do NOT consume newlines when evaluating Expression.Many()
			select x;
	
	//Statements
	public readonly static TokenListParser<SToken, Statement> NewLine =
		from s in Token.EqualTo(SToken.Newline).AtLeastOnce()
		select AST.Statement.Empty;
	static TokenListParser<SToken, Header> Header { get; }= 
		from _lb in Token.EqualTo(SToken.LBrace)
		from id in Identifier
		from _rb in Token.EqualTo(SToken.RBrace)
		select new Header(((Identifier)id).Value);

	public static TokenListParser<SToken, Statement> ModuleDeclaration { get; }=
		from id in Identifier.Try()
		from _ in Token.EqualTo(SToken.Colon)
		from parameters in AtIdentifier.Many()
		from module in Statement
		select (Statement)new ModuleDeclaration((Identifier)id,module,parameters.Cast<Identifier>().ToArray());

	public static TokenListParser<SToken, Statement> VariableDeclaration { get; } =
		from id in Identifier.Try()
		from _ in Token.EqualTo(SToken.Equals)
		from expr in Expression
		select (Statement)new VariableDeclaration((Identifier)id, expr);
	
	public static TokenListParser<SToken, Statement> StackBlock { get; } =
		// from a in NewLine.IgnoreMany()//some type bs making ignoreMany not work. this can be improved.
		from nl1 in NewLine.Many()
		from l in Token.EqualTo(SToken.LBracket)
		from nl2 in NewLine.Many()
		from stmts in Statement.Many()
		from r in Token.EqualTo(SToken.RBracket)
		from nl3 in NewLine.Many()
		select (Statement)new AST.StackBlock(stmts);

	public static TokenListParser<SToken, Statement> GlobalsDeclare { get; } =
		from _ in Token.EqualTo(SToken.Global)
		from ids in Identifier.Many()
		select (Statement)new GlobalsDeclaration(ids);
	public static TokenListParser<SToken, (Identifier id, Expression[] args)> FunctionCallFirstPart { get; } =
		from id in Identifier
		from exprs in Expression.Many()
		select ((Identifier)id, exprs);
		
	public static TokenListParser<SToken, Statement> FunctionCallWithBlock { get; } =
		from fn in FunctionCallFirstPart.Try()//Try allows us to succeed at parsing by trying with block before without block.
		from sb in StackBlock.Try()
		select (Statement)new FunctionCall(fn.id, fn.args, (StackBlock)sb);

	public static TokenListParser<SToken, Statement> FunctionCallNoBlock { get; } =
		from fn in FunctionCallFirstPart
		from _ in NewLine!.OptionalOrDefault(null)
		select (Statement)new FunctionCall(fn.id, fn.args);
	
	//'pushable' right now is just declarations i guess. groups and stuff later?
	public static TokenListParser<SToken, Statement> Push { get; } =
		from _ in Token.EqualTo(SToken.ChevRight)
		select (Statement)new Push();
	
	static TokenListParser<SToken, Statement> Pop { get; } =
		from _ in Token.EqualTo(SToken.Dot)
		select (Statement)new Pop();

	public static TokenListParser<SToken, Statement> MetaStatement { get; } =
		from octo in Token.EqualTo(SToken.Octothorpe)
		from s in VariableDeclaration
		select (Statement)new MetaStatement(s);
	public static TokenListParser<SToken, Statement> Statement { get; } =
		from _1 in NewLine.Many()
		from s in Push.Try()
			.Or(Pop)
			//I think technically this is slower than if we seperated function+exprs and then added newline or block
			.Or(ModuleDeclaration.Try()) //has to be after Push, since they both look for Dec first.
			.Or(VariableDeclaration.Try())
			.Or(FunctionCallWithBlock.Try())
			.Or(FunctionCallNoBlock.Try())
			.Or(StackBlock.Try())
			.Or(GlobalsDeclare.Try())
			.Or(MetaStatement.Try())
		from _2 in NewLine.Many()

		select s;
	public static TokenListParser<SToken, Section> Section { get; }=
		from _ in NewLine.Many()
		from h in Header
		from stmnts in Statement.Many()
		select new Section(h, stmnts);

	//todo: implicit meta section? 
	static TokenListParser<SToken, Root> Root { get; } =
		from sections in Section.Many()
		select new Root(sections);

	private static TokenListParser<SToken, Root> Program { get; } = Root.AtEnd().Named("Program");
	
	//todo: return Result
	public static bool TryParse(string input, [MaybeNullWhen(false)] out Root root, [MaybeNullWhen(true)]out string error)
	{
		//hackily add a newline to the end.
		var tokr = ShapeTokenizer.Tokenizer.TryTokenize(input+Environment.NewLine);
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

