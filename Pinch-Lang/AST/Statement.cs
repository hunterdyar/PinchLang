using ShapesDeclare.Utility;
using Superpower.Model;

namespace ShapesDeclare.AST;

public class Statement
{
	public static Statement Empty = new Statement();
}

public class MetaStatement : Statement
{
	public Statement Statement => _statement;
	private readonly Statement _statement;

	public MetaStatement(Statement statement)
	{
		_statement = statement;
	}

	public override string ToString()
	{
		return "#" + _statement.ToString();
	}
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
public class ModuleDeclaration : Statement
{
	//Name:ShapeType listOfProperties
	public Identifier Name;
	public Statement Statement;
	public Identifier[] Params;
	
	//banana: @a @b Statement

	public ModuleDeclaration(Identifier id, Statement module, Identifier[] parameters)
	{
		Name = id;
		Statement = module;
		Params = parameters;

		if (id.Prefix != IdentPrefix.None)
		{
			//register warning...
			throw new Exception(
				$"Module Declarations (name: @p statement) should not have prefixed identifiers ({id.Prefix}){id.Value.ToString()}). Prefix is ignored.");
			id.Prefix = IdentPrefix.None;
		}
	}

	public override string ToString()
	{
		return Name + ":" + (Params.Length > 0 ? " " : "") + Params.ToStringDelimited(" ") + " " +Statement.ToString();
	}
}

public class VariableDeclaration : Statement
{
	//Name:ShapeType listOfProperties
	public readonly Identifier Name;
	public readonly Expression Expression;
	//banana: @a @b Statement

	public VariableDeclaration(Identifier id, Expression expression)
	{
		Name = id;
		Expression = expression;

		if (id.Prefix != IdentPrefix.None)
		{
			//register warning...
			throw new Exception(
				$"Variable Declarations (name = expression) should not have prefixed identifiers ({id.Prefix}){id.Value.ToString()}). Prefix is ignored.");
			id.Prefix = IdentPrefix.None;
		}
	}

	public override string ToString()
	{
		return Name + " = " + Expression.ToString();
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
	public override string ToString()
	{
		return ">";
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


