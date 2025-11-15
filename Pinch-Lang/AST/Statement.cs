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
	public Identifier Name;
	public Identifier[] Properties;
	public Statement Body;

	public NamedStatement(Identifier name, Identifier[] properties, Statement body)
	{
		Name = name;
		Properties = properties;
		Body = body;
	}
	
	public override string ToString()
	{
		return Name + ":" + (Properties.Length > 0 ? " " : "") + Properties.ToStringDelimited(" ") + " "+Body.ToString();
	}
}

//was "c1:circle 20 20"
//now its "circle 10 10", which is a function....
//so how does push know what to put on the stack? context! Takes the most recently evaluated Shape and puts it on a stack?
//and 'most recently' here can be pretty strict cus we can just clear the context when appropriate.
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

	public FunctionCall(Identifier id,  Expression[] arguments)
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

public class GlobalsDeclaration : Statement
{
	public Identifier[] Identifiers => _ids;
	private readonly Identifier[] _ids;
	public GlobalsDeclaration(Expression[] ids)
	{
		_ids = new Identifier[ids.Length];
		for (int i = 0; i < ids.Length; i++)
		{
			if (ids[i] is Identifier id)
			{
				_ids[i] = id;
			}
			else
			{
				throw new Exception("properties to 'global' must be identifiers.");
			}
		}

		if (ids.Length == 0)
		{
			throw new Exception("setting variables as 'global' must take at least one identifier.");
		}
	}
}


