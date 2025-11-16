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

	public static void ConvexHull(Environment env, ValueItem[] ags, List<StackItem> items)
	{
		Builtins.ValidateArgumentCount("convex hull",0, []);
		var g = Group(env, items);
		var p = g.ConvexHull();
		PushGeometryToStack(env, p);
	}

	public static void Difference(Environment env, ValueItem[] ags, List<StackItem> items)
	{
		Builtins.ValidateArgumentCount("difference", 0, []);
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

		var main = list[0].GetGeometry();
		for (int i = 1; i < list.Length; i++)
		{
			main = main.Difference(list[i].GetGeometry());
		}
		
		PushGeometryToStack(env, main);
	}

	static void PushGeometryToStack(Environment env, Geometry geo)
	{
		if (geo is Polygon polygon)
		{
			var si = new Poly(env, polygon);
			env.Push(si);
		}else if (geo is GeometryCollection geometryCollection)
		{
			throw new NotImplementedException("Geometry Collection item not supported yet");
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