using NetTopologySuite.Geometries;
using NetTopologySuite.Shape;
using Pinch_Lang.Walker;
using ShapesDeclare.AST;
using Svg;

namespace Pinch_Lang.Engine;

public abstract class StackItem
{
	protected Environment _environment;

	protected StackItem(Environment env)
	{
		_environment = env;
	}

	public virtual void SetProperty(string propName, Expression expression)
	{
		var item = _environment.ExprWalker.WalkExpression(expression);
		SetProperty(propName, item);
	}

	public abstract void SetProperty(string propName, ValueItem item);

}

public abstract class Shape : StackItem
{
	protected Shape(Environment env) : base(env)
	{
	}

	protected virtual bool ShapeHasProperty()
	{
		return false;
	}

	public abstract Geometry GetGeometry();

	
	public abstract void RenderToSVGParent(ref SvgElementCollection parent);
}


public class Rect : Shape
{
	private Coordinate _min;
	private Coordinate _max;
	public Rect(Environment env, Coordinate min, Coordinate max) : base(env)
	{
		_min = min;
		_max = max;
	}

	public override Geometry GetGeometry()
	{
		return Geometry.DefaultFactory.CreatePolygon([
			_min, new Coordinate(_min.X, _max.Y), _max, new Coordinate(_max.X, _min.Y)
		]);
	}

	public override void SetProperty(string propName, ValueItem item)
	{
		// if (propName == "radius")
		// {
		// 	var r = ValueItem.AsNumber(item);
		// 	_shape.Radius = (float)r;
		// }else if (propName == "center_x")
		// {
		// 	var cx = ValueItem.AsNumber(item);
		// 	_shape.Center = new Point2f(cx, _shape.Center.y);
		// }
		// else if (propName == "center_y")
		// {
		// 	var cy = ValueItem.AsNumber(item);
		// 	_shape.Center = new Point2f(_shape.Center.x, cy);
		// }
	}

	public override void RenderToSVGParent(ref SvgElementCollection parent)
	{
		var c = new SvgRectangle()
		{
			X = new SvgUnit(SvgUnitType.None,(float)_min.X),
			Y = new SvgUnit(SvgUnitType.None,(float)_min.Y),
			Width = new SvgUnit(SvgUnitType.None, (float)(_max.X-_min.X)),
			Height = new SvgUnit(SvgUnitType.None, (float)(_max.Y - _min.Y)),
		};
		
		parent.Add(c);
	}
	
}