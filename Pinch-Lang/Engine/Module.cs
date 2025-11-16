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
		
		//todo: Statement is probably a StackBlock, and those push their own frame.
		//we also push a frame to hold the locals. Instead, we should 'inject' the locals into the stack's frame.
		//this technically works, so i'm putting to the side for now since things might get totally re-written there anyway.
		var frame = env.PushNewFrame();
		
		//assign the 
		if (a.Length > 0)
		{
			for (int i = 0; i < a.Length; i++)
			{
				frame.SetLocal(Parameters[i].Value.ToString(), a[i]);
			}
		}
		
		env.StatementWalker.WalkStatement(Statement);
		
		//the exits aren't quite working here.... the extra frame  above messing us up?
		var f2 =env.PopFrame();
		f2.ExitFrame();
	}
}