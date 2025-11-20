using NetTopologySuite.Shape;
using NetTopologySuite.Utilities;
using Pinch_Lang.Walker;
using ShapesDeclare.AST;
using Svg;

namespace Pinch_Lang.Engine;

public struct CanvasProperties
{
	public double Width = 100;
	public double Height = 100;
	public CanvasProperties()
	{
	}
}

public enum SectionType
{
	Regular,
	CanvasProperties,
	Ignore,
}
public class Environment
{
	public Frame RootFrame;
	private Stack<Frame> _frames = new Stack<Frame>();
	private readonly List<StackItem> _canvas = new List<StackItem>();
	public Frame CurrentFrame => GetCurrentFrame();
	public bool IsAtRootFrame => _frames.Count == 0;

	public readonly Walker.StatementWalker StatementWalker;
	public readonly ExpressionWalker ExprWalker;
	public CanvasProperties CanvasProperties;
	private string _currentSection = "";

	public SectionType SectionType => _sectionType;
	private SectionType _sectionType;
	public Environment()
	{
		StatementWalker = new Walker.StatementWalker(this);
		ExprWalker = new ExpressionWalker(this);
		RootFrame = new Frame(this);
		CanvasProperties = new CanvasProperties();
		_sectionType = SectionType.Regular;
	}

	public Result Execute(Root root)
	{
		try
		{
			StatementWalker.Walk(root);
			//now, we should have a representation of our shape on the stack.
			SetSection(""); //this will shift items to canvas if [canvas] is the last (current) section.
			var doc = SVGRendering.RenderSVGFromStack(CanvasProperties,_canvas);
			return Result.Success(doc);
		}
		catch (Exception e)
		{
			return Result.GetErrorResult(e);
		}
	}

	private Frame GetCurrentFrame()
	{
		if (_frames.Count == 0)
		{
			return RootFrame;
		}
		else
		{
			var f = _frames.Peek();
			return f;
		}
	}
	public void Push(StackItem item)
	{
		CurrentFrame.PushStackItem(item);
	}

	public void Pop()
	{
		CurrentFrame.PopStackItem();
	}

	public Frame PushNewFrame()
	{
		var f = new Frame(this);
		_frames.Push(f);
		return f;
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
		if (_currentSection == "canvas")
		{
			var add = CurrentFrame.GetStack();
			_canvas.AddRange(add);
		}

		title = title.ToLowerInvariant();
		if (title == "ignore")
		{
			_sectionType = SectionType.Ignore;
		}else if (title == "properties")
		{
			_sectionType = SectionType.CanvasProperties;
		}
		else
		{
			_sectionType = SectionType.Regular;
		}
		
		_currentSection = title;
		CurrentFrame.ClearStack();
		
		//this was important for later features, where code is split into different parts where the root stack means different things
		//like [canvas] draws whatever is on the stack to the canvas, while other sections don't.
		//or [test]
		//or [comments] which don't get processed.

		//clear the last frame.
	}

	public Module RegisterModule(string modName, Identifier[] args, Statement statement)
	{
		Module m = new Module(CurrentFrame, modName, args, statement);
		CurrentFrame.RegisterModule(m);
		return m;
	}

	public bool TryGetModule(string name, out Module module)
	{
		return CurrentFrame.TryGetModule(name, out module);
	}

	public void SetMetaProperty(Identifier id, ValueItem val)
	{
		if (id.Prefix != IdentPrefix.None)
		{
			throw new Exception($"Meta Property assignment cannot have prefixes. ({id})");
		}

		var prop = id.Value.ToString();
		switch (prop)
		{
			case "width":
				CanvasProperties.Width = val.AsNumber();
				break;
			case "height":
				CanvasProperties.Height = val.AsNumber();
				break;
			default:
				throw new Exception($"Unknown Meta Property '{prop}'");
		}
	}
}