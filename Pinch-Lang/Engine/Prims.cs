using NetTopologySuite.Geometries;
using NetTopologySuite.Utilities;
using Svg;

namespace Pinch_Lang.Engine;


public class Rect : Shape
{
	private Polygon _polygon;

	public Rect(Environment env, Coordinate min, Coordinate max) : base(env)
	{
		_polygon = Geometry.DefaultFactory.CreatePolygon([
			min, new Coordinate(min.X, max.Y), max, new Coordinate(max.X, min.Y),
			min
		]);

		Assert.IsTrue(_polygon.IsRectangle);
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
		var e = _polygon.RenderToSVGElement();
		parent.Add(e);
	}

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
		Console.WriteLine($"Rendering circle without using geometry: {_center}, {_radius}");

		var c = new SvgCircle()
		{
			CenterX = new SvgUnit(SvgUnitType.None, (float)_center.X),
			CenterY = new SvgUnit(SvgUnitType.None, (float)_center.Y),
			Radius = new SvgUnit(SvgUnitType.None, (float)_radius),
		};
		parent.Add(c);
	}

	public override void SetProperty(string propName, ValueItem item)
	{
		switch (propName)
		{
			case "radius":
				var r = item.AsNumber();
				_radius = r;
				break;
			case "centerx":
				var cx = item.AsNumber();
				_center.X = cx;
				break;
			case "centery":
				var cy = item.AsNumber();
				_center.Y = cy;
				break;
			default:
				throw new Exception($"Invalid Argument {item} for Circle {propName}");
		}
	}
}