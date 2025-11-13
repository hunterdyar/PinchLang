using System.Globalization;
using Superpower.Model;

namespace ShapesDeclare.AST;

public class Expression
{
	
}

public class Identifier : Expression
{
	public TextSpan Value;
	public Identifier(TextSpan idSpan)
	{
		Value = idSpan;
	}

	public override string ToString()
	{
		return Value.ToString();
	}
}

public class ThrowawayIdentifier : Identifier
{
	public ThrowawayIdentifier(TextSpan idSpan) : base(idSpan)
	{
		
	}

	public override string ToString()
	{
		return "_";
	}
}

public class Literal : Expression
{
	
}

public class NumberLiteral : Literal
{
	public double Value;

	public NumberLiteral(double val)
	{
		Value = val;
	}

	public NumberLiteral(int val)
	{
		Value = val;
	}

	public NumberLiteral(float val)
	{
		Value = val;
	}

	public NumberLiteral(TextSpan span)
	{
		Value = double.Parse(span.ToString());
	}

	public override string ToString()
	{
		return Value.ToString(CultureInfo.InvariantCulture);
	}
}
public class StringLiteral : Literal
{
	public TextSpan Value;

	public StringLiteral(TextSpan val)
	{
		Value = val;
	}

	public override string ToString()
	{
		return Value.ToString(); 
	}
}
public class IntegerLiteral : Literal
{
	public TextSpan TextValue;
	public int Value;

	public IntegerLiteral(TextSpan val)
	{
		TextValue = val;
		Value = int.Parse(val.ToString());
	}

	public override string ToString()
	{
		return Value.ToString();
	}
}

public class DoubleLiteral : Literal
{
	public TextSpan TextValue;
	public double Value;

	public DoubleLiteral(TextSpan val)
	{
		TextValue = val;
		Value = double.Parse(val.ToString());
	}

	public override string ToString()
	{
		return Value.ToString(CultureInfo.InvariantCulture);
	}
}

public class Operator : Expression
{
	
}

public class UnaryOperator(Token<SToken> op, Expression exp) : Operator
{
	//todo
}
public class BinaryOperator : Operator
{
	public Expression Left;
	public Expression Right;
	public BinOp Op;
}

public class Tuple<TA, TB> : Expression
{
	public TA A;
	public TB B;
}
public class IDTuple : Tuple<Identifier, Identifier>
{
	public IDTuple(Identifier left, Identifier right)
	{
		A = left;
		B = right;
	}

	public override string ToString()
	{
		return A + ":" + B;
	}
}

public class ExprTuple : Tuple<Expression, Expression>
{
	public ExprTuple(Expression left, Expression right)
	{
		A = left;
		B = right;
	}

	public override string ToString()
	{
		return A + ":" + B;
	}
}