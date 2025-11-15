using ShapesDeclare.AST;

namespace Pinch_Lang.Engine;

public class Module
{
	public readonly string Name;
	public Identifier[] Parameters;
	public readonly Statement Statement;
	private Frame _frame;
	public Module(Frame frame, string name, Identifier[] parameters, Statement statement)
	{
		_frame = frame;
		Name = name;
		Parameters = parameters;
		Statement = statement;
	}
}