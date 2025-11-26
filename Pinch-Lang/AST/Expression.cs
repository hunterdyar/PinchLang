using System.Globalization;
using Superpower.Model;

namespace ShapesDeclare.AST;

public enum IdentPrefix
{
	None,
	Dot,
	Underscore,
	At,
	Bang,
	Octothorpe
}
public class Expression
{
	
}

public class Identifier : Expression
{
	public TextSpan Value;
	public  IdentPrefix Prefix = IdentPrefix.None;
	public Identifier(TextSpan idSpan)
	{
		Value = idSpan;
		Prefix = IdentPrefix.None;
	}
	public Identifier(TextSpan idSpan, IdentPrefix prefix)
	{
		//save the id not the _ or . or whatever. its awkward because this is something where we dont ignore whitespace, the . has to be connected.
		//if we write our own lexer, this could be different.
		if (prefix != IdentPrefix.None)
		{
			Value = idSpan.Skip(1);
		}
		else
		{
			Value = idSpan;
		}
		
		Prefix = prefix;
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


public class FunctionExpressionCall : Expression
{
	public Identifier Name => _id;
	private Identifier _id;
	public Expression[] Arguments => _args;
	private Expression[] _args;
	public FunctionExpressionCall(Identifier id, Expression[] args)
	{
		_id = id;
		_args = args;
	}
}


public class Tuple<TA, TB> : Expression
{
	public TA A;
	public TB B;
}

public class KeyValueTuple : Tuple<Identifier, Expression>
{
	public KeyValueTuple(Identifier left, Expression right)
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