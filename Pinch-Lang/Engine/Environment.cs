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
	public bool IsAtRootFrame => _frames.Count == 0;

	public readonly Walker.StatementWalker StatementWalker;
	public readonly ExpressionWalker ExprWalker;
	
	public Environment()
	{
		StatementWalker = new Walker.StatementWalker(this);
		ExprWalker = new ExpressionWalker(this);
		RootFrame = new Frame(this);
	}

	public SvgDocument Execute(Root root)
	{
		StatementWalker.Walk(root);
		//now, we should have a representation of our shape on the stack.
		var canvas = CurrentFrame.GetStack();
		return SVGRendering.RenderSVGFromStack(canvas);
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
		_frames.Push(new Frame(this));
	}

	public Frame PopFrame()
	{
		if (_frames.Count > 0)
		{
			return _frames.Pop();
		}
		else
		{
			throw new Exception("Can't pop frame, down to root frame");
		}
	}

	public void SetSection(string title)
	{
		//this was important for later features, where code is split into different parts where the root stack means different things
		//like [canvas] draws whatever is on the stack to the canvas, while other sections don't.
		//or [test]
		//or [comments] which don't get processed.
	}

	public Module RegisterModule(string modName, Identifier[] args, Statement statement)
	{
		Module m = new Module(CurrentFrame, modName, args, statement);
		CurrentFrame.RegisterModule(m);
		return m;
	}
}