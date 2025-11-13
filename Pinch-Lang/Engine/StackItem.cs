using NetTopologySuite.Geometries;
using NetTopologySuite.Shape;
using NetTopologySuite.Utilities;
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

public class Circle : Shape
{
	//todo:remove coord and center and replace with factory wrapper.
	private Coordinate _center;
	private double _radius;
	private GeometricShapeFactory _factory = new GeometricShapeFactory();

	public Circle(Environment env, Coordinate center, double radius) : base(env)
	{
		_center = center;
		_radius = radius;
	}

	public override Geometry GetGeometry()
	{
		_factory.Width = _radius * 2;
		_factory.Height = _radius * 2;
		_factory.Centre = _center;
		
		return _factory.CreateCircle();
	}

	public override void RenderToSVGParent(ref SvgElementCollection parent)
	{
		throw new NotImplementedException();
	}

	public override void SetProperty(string propName, ValueItem item)
	{
		throw new NotImplementedException();
	}
}
public class Rect : Shape
{
	private Polygon _polygon;
	public Rect(Environment env, Coordinate min, Coordinate max) : base(env)
	{
		_polygon = Geometry.DefaultFactory.CreatePolygon([
			min, new Coordinate(min.X, max.Y), max, new Coordinate(max.X, min.Y)
		]);
	}

	public override Geometry GetGeometry()
	{
		return _polygon;
	}

	public override void SetProperty(string propName, ValueItem item)
	{
		throw new NotImplementedException();
	}

	public override void RenderToSVGParent(ref SvgElementCollection parent)
	{
		
		// var r = new SvgPolygon()
		// {
		// 	Points	= points
		// }
		// var c = new SvgRectangle()
		// {
		// 	X = new SvgUnit(SvgUnitType.None,(float)min.X),
		// 	Y = new SvgUnit(SvgUnitType.None,(float)min.Y),
		// 	Width = new SvgUnit(SvgUnitType.None, (float)(_max.X-_min.X)),
		// 	Height = new SvgUnit(SvgUnitType.None, (float)(_max.Y - _min.Y)),
		// };
		
		//parent.Add(c);
	}
	
}