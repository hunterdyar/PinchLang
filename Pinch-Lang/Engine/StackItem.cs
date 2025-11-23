using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Utilities;
using NetTopologySuite.Shape;
using NetTopologySuite.Utilities;
using Pinch_Lang.Walker;
using ShapesDeclare.AST;
using Svg;
using Svg.Pathing;

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

	/// <summary>
	/// returns the geometry, but if there's a GeometryCollection, it applies Union to it first.
	/// </summary>
	public Geometry GetSingleGeometry()
	{
		//
		//todo: this should use GeomeryTransformer (Combiner?) factory isntead, NetTopologySuite has builtins for this feature: https://nettopologysuite.github.io/NetTopologySuite/api/NetTopologySuite.Geometries.Utilities.GeometryTransformer.html
		var g = GetGeometry();
		if (g is GeometryCollection gc)
		{
			g = gc.Union();
		}

		return g;
	}

	public abstract void AffineTransform(AffineTransformation gt);
}

public class Poly : Shape
{
	public Polygon Polygon => _polygon;
	private Polygon _polygon;
	public Poly(Environment env, Polygon poly) : base(env)
	{
		_polygon = poly;
	}

	public override void SetProperty(string propName, ValueItem item)
	{
		throw new NotImplementedException();
	}

	public override Geometry GetGeometry()
	{
		return _polygon;
	}

	public override void RenderToSVGParent(ref SvgElementCollection parent)
	{
		var p = _polygon.RenderToSVGElement();
		parent.Add(p);
	}

	public override void AffineTransform(AffineTransformation gt)
	{
		_polygon = gt.Transform(_polygon) as Polygon ?? throw new InvalidOperationException();
	}
}

public class PolyGroup : Shape
{
	public GeometryCollection Collection => _collection;
	private GeometryCollection _collection;

	public PolyGroup(Environment env, GeometryCollection collection) : base(env)
	{
		_collection = collection;
	}

	public PolyGroup(Environment env, List<StackItem> items) : base(env)
	{
		var j = items.Cast<Shape>().Select(x=>x.GetGeometry());
		_collection = new GeometryCollection(j.ToArray());
	}

	public override void SetProperty(string propName, ValueItem item)
	{
		throw new NotImplementedException();
	}

	public override Geometry GetGeometry()
	{
		return _collection;
	}

	public override void RenderToSVGParent(ref SvgElementCollection parent)
	{
		var p = _collection.RenderToSVGElement();
		parent.Add(p);
	}

	public override void AffineTransform(AffineTransformation transformation)
	{
		_collection = transformation.Transform(_collection) as GeometryCollection ?? throw new InvalidOperationException();
	}
}

public class PolyPoint : Shape
{
	public Point Point => _point;
	private Point _point;

	public PolyPoint(Environment env, Point collection) : base(env)
	{
		_point = collection;
	}

	public override void SetProperty(string propName, ValueItem item)
	{
		throw new NotImplementedException();
	}

	public override Geometry GetGeometry()
	{
		return _point;
	}

	public override void RenderToSVGParent(ref SvgElementCollection parent)
	{
		var p = _point.RenderToSVGElement();
		parent.Add(p);
	}

	public override void AffineTransform(AffineTransformation gt)
	{
		_point = gt.Transform(_point) as Point ?? throw new InvalidOperationException();
	}
}
