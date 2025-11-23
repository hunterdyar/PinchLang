using NetTopologySuite.Geometries;
using Pinch_Lang.Engine;
using Environment = Pinch_Lang.Engine.Environment;

namespace Pinch_Lang.Walker;

public static class GeoProcessing
{
	public static Geometry Group(Environment env, List<StackItem> items)
	{
		List<Geometry> polygons = new List<Geometry>();
		foreach (var item in items)
		{
			if (item is Shape shape)
			{
				polygons.Add(shape.GetGeometry());
			}
		}
		
		GeometryCollection gc = Geometry.DefaultFactory.CreateGeometryCollection(polygons.ToArray());
		return gc;
	}

	public static void ConvexHull(Environment env, ValueItem[] args, List<StackItem> items)
	{
		Builtins.ValidateArgumentCount("convex hull",0, []);
		var g = Group(env, items);
		var p = g.ConvexHull();
		PushGeometryToStack(env, p);
	}

	public static void Difference(Environment env, ValueItem[] args, List<StackItem> items)
	{
		Builtins.ValidateArgumentCount("difference", args.Length, []);
		var list = items.Cast<Shape>().ToArray();
		if (list.Length == 0)
		{
			throw new Exception("Can't take difference, need at least one shape item");
		}
		if (list.Length == 1)
		{
			env.Push(items[0]);
			return;
		}

		var main = list[0].GetSingleGeometry();
		
		for (int i = 1; i < list.Length; i++)
		{
			var l = list[i].GetSingleGeometry();
			main = main.Difference(l);
		}
		
		PushGeometryToStack(env, main);
	}

	public static void Intersect(Environment env, ValueItem[] args, List<StackItem> items)
	{
		Builtins.ValidateArgumentCount("intersect", args.Length, []);
		var list = items.Cast<Shape>().ToArray();
		if (list.Length == 0)
		{
			throw new Exception("Can't take intersection, need at least one shape item");
		}

		if (list.Length == 1)
		{
			env.Push(items[0]);
			return;
		}

		var main = list[0].GetSingleGeometry();

		for (int i = 1; i < list.Length; i++)
		{
			var l = list[i].GetSingleGeometry();
			main = main.Intersection(l);
		}

		PushGeometryToStack(env, main);
	}
	
	public static void Buffer(Environment env, ValueItem[] args, List<StackItem> items)
	{
		Builtins.ValidateArgumentCount("buffer", args.Length, [["buffer_dist"]]);
		var dist = args[0].AsNumber();

		var list = items.Cast<Shape>().ToArray();
		if (list.Length == 1)
		{
			var main = list[0].GetSingleGeometry();
			main = main.Buffer(dist);
			PushGeometryToStack(env, main);
		}else if (list.Length == 0)
		{
			if (env.CurrentFrame.PopStackItem() is Shape context)
			{
				var main = context.GetSingleGeometry();
				main = main.Buffer(dist);
				PushGeometryToStack(env,main);
			}
			else
			{
				throw new Exception("Buffer called with no context; using stack. Invalid item on stack.");
			}
		}
		else
		{
			throw new Exception("Invalid number of stack items provided to buffer. Needs O (uses top of stack) or 1 (pushed buffered to stack)");
		}
	}
	
	static void PushGeometryToStack(Environment env, Geometry geo)
	{
		if (geo is Polygon polygon)
		{
			var si = new Poly(env, polygon);
			env.Push(si);
		}else if (geo is GeometryCollection geometryCollection)
		{
			var si = new PolyGroup(env, geometryCollection);
			env.Push(si);
		}else if (geo is LineString lineString)
		{
			throw new NotImplementedException("lineString item not supported yet");
		}else if (geo is Point point)
		{
			throw new NotImplementedException("pure point item not supported yet");
		}
		else
		{
			throw new Exception($"unhandled Geometry {geo}");
		}
	}
}