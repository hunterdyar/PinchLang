using System.Diagnostics;
using ShapesDeclare.AST;

namespace Pinch_Lang.Engine;

public class Module
{
	public readonly string Name;
	public readonly int ParamCount;
	public readonly Identifier[] Parameters;
	public readonly Statement Statement;
	private readonly Frame _registeredFrame;
	private Environment env => _registeredFrame.Environment;
	public Module(Frame registeredFrame, string name, Identifier[] parameters, Statement statement)
	{
		_registeredFrame = registeredFrame;
		Name = name;
		Parameters = parameters;
		Statement = statement;
		ParamCount = Parameters.Length;
	}

	public void Walk(Expression[] args)
	{
		if (args.Length != Parameters.Length)
		{
			throw new Exception(
				$"Incorrect number of arguments for mod {Name}. Got {args.Length}, expected {Parameters.Length}");
		}

		var a = env.StatementWalker.WalkExpressionListToItemList(args);
		
		var frame = env.PushNewFrame();
		
		//assign the 
		if (a.Length > 0)
		{
			for (int i = 0; i < a.Length; i++)
			{
				frame.SetLocal(Parameters[i].Value.ToString(), a[i]);
			}
		}

		//calling WalkStatement will push and pop a frame... but WE want to push and pop a frame to serve as context for the function arguments.
		//so let's skip that and do it ourselves here.
		if (Statement is StackBlock sb)
		{
			foreach (var statement in sb.Statements)
			{
				env.StatementWalker.WalkStatement(statement);
			}
		}
		else
		{
			env.StatementWalker.WalkStatement(Statement);
		}

		//the exits aren't quite working here.... the extra frame  above messing us up?
		var f2 =env.PopFrame();
		f2.ExitFrame();
	}
}