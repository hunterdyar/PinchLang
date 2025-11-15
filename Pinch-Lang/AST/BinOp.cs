namespace ShapesDeclare.AST;

// public enum BinOp
// {
// 	Plus,
// 	Minus,
// 	Times,
// 	Divide,
// 	Modulo,
// 	Pow,
// }

public class BinaryOperator : Operator
{
	public Expression Left;
	public Expression Right;

	public BinaryOperator(Expression left, Expression right)
	{
		this.Left = left;
		this.Right = right;
	}
	public static BinaryOperator CreateBinaryOp(SToken opToken, Expression left, Expression right)
	{
		switch (opToken)
		{
			case SToken.Plus:
				return new Plus(left, right);
			case SToken.Minus:
				return new Minus(left, right);
			case SToken.Asterisk:
				return new Times(left, right);
			case SToken.Slash:
				return new Divide(left, right);
			case SToken.Percentage:
				return new Modulo(left, right);
			default:
				throw new Exception($"Unknown Binary Operator: {opToken}");
		}
	}
}

public class Plus : BinaryOperator
{
	public Plus(Expression left, Expression right) : base(left, right)
	{
	}
}
public class Minus : BinaryOperator
{
	public Minus(Expression left, Expression right) : base(left, right)
	{
	}
}

public class Times : BinaryOperator
{
	public Times(Expression left, Expression right) : base(left, right)
	{
	}
}

public class Divide : BinaryOperator
{
	public Divide(Expression left, Expression right) : base(left, right)
	{
	}
}

public class Modulo : BinaryOperator
{
	public Modulo(Expression left, Expression right) : base(left, right)
	{
	}
}

