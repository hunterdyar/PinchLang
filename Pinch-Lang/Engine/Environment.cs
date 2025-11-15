using NetTopologySuite.Shape;
using NetTopologySuite.Utilities;
using Pinch_Lang.Walker;
using ShapesDeclare.AST;
using Svg;

namespace Pinch_Lang.Engine;

public class Environment
{
	public Frame RootFrame;
	private Stack<Frame> _frames = new Stack<Frame>();
	public Frame CurrentFrame => _frames.Count > 0 ? _frames.Peek() : RootFrame;
	
	public readonly Walker.StatementWalker StatementWalker;
	public readonly ExpressionWalker ExprWalker;



	
	public Environment()
	{
		StatementWalker = new Walker.StatementWalker(this);
		ExprWalker = new ExpressionWalker(this);
		RootFrame = new Frame();
	}

	public void Execute(Root root)
	{
		StatementWalker.Walk(root);
	}

	public void Push(StackItem item)
	{
		CurrentFrame.PushStackItem(item);
	}

	public void Pop()
	{
		CurrentFrame.PopStackItem();
	}

	public void PushNewFrame()
	{
		_frames.Push(new Frame());
	}

	public void PopFrame()
	{
		if (_frames.Count > 0)
		{
			_frames.Pop();
		}
		else
		{
			throw new Exception("Can't pop frame, down to root frame");
		}
	}

	//todo: this will be elsewhere.
	public SvgDocument RenderSVG()
	{
		SvgDocument doc = new SvgDocument();
		SvgElementCollection parent = doc.Children;
		// foreach (var kvp in Declarations)
		// {
		// 	var val = kvp.Value;
		// 	if (val is Shape shape)
		// 	{
		// 		shape.RenderToSVGParent(ref parent);
		// 	}
		// }

		return doc;
	}
}