using System.Linq.Expressions;
using ShapesDeclare.AST;
using Superpower;
using Superpower.Parsers;
using Expression = ShapesDeclare.AST.Expression;

namespace ShapesDeclare;

public static class ExprParser
{
	public static TokenListParser<SToken, OpItem> Multiply { get; } = Token.EqualTo(SToken.Asterisk).Value(new OpItem(BinOp.Times));
	public static TokenListParser<SToken, OpItem> Divide { get; } = Token.EqualTo(SToken.Slash).Value(new OpItem(BinOp.Divide));
	public static TokenListParser<SToken, OpItem> Plus { get; } = Token.EqualTo(SToken.Plus).Value(new OpItem(BinOp.Plus));
	public static TokenListParser<SToken, OpItem> Minus { get; } = Token.EqualTo(SToken.Minus).Value(new OpItem(BinOp.Minus));
	public static TokenListParser<SToken, OpItem> Modulo { get; } = Token.EqualTo(SToken.Percentage).Value(new OpItem(BinOp.Modulo));


		public static TokenListParser<SToken, Expression> NumberLiteral { get; } =
			from num in Token.EqualTo(SToken.Integer).Or(Token.EqualTo(SToken.Double))
			select (Expression)new NumberLiteral(num.Span);

		public static TokenListParser<SToken, Expression> StringLiteral { get; } =
		from str in Token.EqualTo(SToken.String)
		select (Expression)new StringLiteral(str.Span);

		public static TokenListParser<SToken, Expression> ListLiteral { get; } =
			from lb in Token.EqualTo(SToken.LBrace)
			from exprs in Parse.Ref(() => Expr).ManyDelimitedBy(Token.EqualTo(SToken.Comma).Optional())
			from rb in Token.EqualTo(SToken.RBrace)
			select (Expression)new ListLiteral(exprs);

		private static TokenListParser<SToken, Expression> Literal { get; } =
			from literal in
				NumberLiteral
					.Or(StringLiteral)
					//.Or(ShapeParser.ExpressionIdentifier)
					.Or(ListLiteral)
			select literal;

        static readonly TokenListParser<SToken, Expression> Factor =
            (from lparen in Token.EqualTo(SToken.LParen)
             from expr in Parse.Ref(() => Expr!)
             from rparen in Token.EqualTo(SToken.RParen)
             select expr)
            .Or(Literal);

        static readonly TokenListParser<SToken, Expression> Operand =
            (from sign in Token.EqualTo(SToken.Minus)
             from factor in Factor
             select (Expression)CreateUnary(new OpItem(BinOp.Minus),factor))
            .Or(Factor).Named("expression");

        private static Expression CreateBinary(OpItem op, Expression left, Expression right)
        {
	        switch (op.Op)
	        {
		        case BinOp.Times: return new Multiply(left, right);
		        case BinOp.Divide: return new Divide(left, right);
		        case BinOp.Minus: return new Minus(left, right);
		        case BinOp.Plus: return new Plus(left, right);
		        // case BinOp.Pow: return new P(left, right);
		        case BinOp.Modulo: return new Modulo(left, right);
		        default:
			        throw new Exception($"unsupported op {op}");
	        }
        }

        private static Expression CreateUnary(OpItem op,Expression right)
        {
	        switch (op.Op)
	        {
		        case BinOp.Minus: return new Negate(right);
		        default:
			        throw new Exception($"unsupported op {op}");
	        }
        }
        
        
        static readonly TokenListParser<SToken, Expression> Term = Parse.Chain(Multiply.Or(Divide).Or(Modulo), Operand, CreateBinary);
        public static readonly TokenListParser<SToken, Expression> Expr = Parse.Chain(Plus.Or(Minus), Term, CreateBinary);
        
}