using ShapesDeclare.Utility;
using Superpower.Model;

namespace ShapesDeclare.AST;

public class Statement
{
	public static Statement Empty = new Statement();
}

public class NamedStatement : Statement
{
	// public TextSpan Name;
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
		throw new NotImplementedException("gotta delete this and make named statements as modules, and standalone blocks.");
	}

	public override string ToString()
	{
		return Name + ":" + ShapeType + (Expressions.Length > 0 ? " " : "") + Expressions.ToStringDelimited(" ");
	}
}

public class FunctionCall : Statement
{
	public int PopFromStack;
	public StackBlock? StackBlock;
	public Identifier Identifier;
	public TextSpan Name => Identifier.Value;
	public Expression[] Arguments;

	public FunctionCall(Identifier id, params Expression[] arguments)
	{
		PopFromStack = CountPopFromID(id);
		Identifier = id;
		Arguments = arguments;
		StackBlock = null;
	}

	public FunctionCall(Identifier id, Expression[] arguments, StackBlock? block)
	{
		Identifier = id;
		Arguments = arguments;
		PopFromStack = CountPopFromID(id);
		StackBlock = block;
	}

	private int CountPopFromID(Identifier id)
	{
		if (id.Prefix == IdentPrefix.Dot)
		{
			return 1;

		}

		return 0;
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



