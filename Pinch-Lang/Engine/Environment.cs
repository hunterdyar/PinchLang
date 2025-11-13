using CGALDotNetGeometry.Shapes;
using Pinch_Lang.Walker;
using ShapesDeclare.AST;

namespace Pinch_Lang.Engine;

public class Environment
{
	private List<IShape2f> _shapes = new List<IShape2f>();

	public Dictionary<string, StackItem> Declarations => _declarations;
	private Dictionary<string, StackItem> _declarations = new Dictionary<string, StackItem>();

	public readonly Walker.StatementWalker StatementWalker;
	public readonly ExpressionWalker ExprWalker;

	public List<StackItem> ShapeStack => _shapeStack;
	private List<StackItem> _shapeStack = new List<StackItem>();
	
	public Environment()
	{
		StatementWalker = new Walker.StatementWalker(this);
		ExprWalker = new ExpressionWalker(this);
	}

	public void Execute(Root root)
	{
		StatementWalker.Walk(root);
	}
	//list of properties, modules, etc variable resolver, etc.
	public void AddShape(IShape2f circle)
	{
		_shapes.Add(circle);	
	}
	
	public void DeclareShape(string shapeName, Shape2D shape)
	{
		if (shapeName == "_")
		{
			//no problemo!
			return;
		}
		
		if (!_declarations.TryAdd(shapeName, shape))
		{
			throw new Exception($"Item with name {shapeName} already exists.");
		}
	}

	public void Push(StackItem item)
	{
		_shapeStack.Add(item);
	}

	public void Pop()
	{
		_shapeStack.RemoveAt(_shapeStack.Count-1);
	}
}