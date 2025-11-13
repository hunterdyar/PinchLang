using ShapesDeclare.Utility;
using Superpower.Model;

namespace ShapesDeclare.AST;

public class Statement
{
	public static Statement Empty = new Statement();
}

public class ShapeDeclaration : Statement
{
	//Name:ShapeType listOfProperties
	public TextSpan Name;
	public TextSpan ShapeType;
	public Expression[] Expressions;

	public ShapeDeclaration(IDTuple id, params Expression[] expressions)
	{
		Name = id.A.Value;
		ShapeType = id.B.Value;
		Expressions = expressions;
	}

	public override string ToString()
	{
		return Name + ":" + ShapeType + (Expressions.Length > 0 ? " " : "") + Expressions.ToStringDelimited(" ");
	}
}

public class FunctionCall : Statement
{
	public TextSpan Name;
	public Expression[] Arguments;

	public FunctionCall(Identifier id, params Expression[] arguments)
	{
		Name = id.Value;
		Arguments = arguments;
	}

	public override string ToString()
	{
		return Name + " " + Arguments.ToStringDelimited(" ");
	}
}

public class Push : Statement
{
	public ShapeDeclaration Shape;

	public Push(ShapeDeclaration sb)
	{
		Shape = sb;
	}

	public override string ToString()
	{
		return Shape.ToString() + " >";
	}
}

public class Pop : Statement
{
	public override string ToString()
	{
		return ".";
	}
}



