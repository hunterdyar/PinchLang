using NetTopologySuite.Shape;
using NetTopologySuite.Utilities;
using Pinch_Lang.Walker;
using ShapesDeclare.AST;
using Svg;

namespace Pinch_Lang.Engine;

public class Environment
{
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
	
	public void DeclareShape(string shapeName, Shape shape)
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

	//todo: this will be elsewhere.
	public SvgDocument RenderSVG()
	{
		SvgDocument doc = new SvgDocument();
		SvgElementCollection parent = doc.Children;
		foreach (var kvp in Declarations)
		{
			var val = kvp.Value;
			if (val is Shape shape)
			{
				shape.RenderToSVGParent(ref parent);
			}
		}

		return doc;
	}
}