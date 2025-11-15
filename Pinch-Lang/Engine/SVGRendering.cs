using NetTopologySuite.Geometries;
using Svg;
using Svg.Pathing;

namespace Pinch_Lang.Engine;

public static class SVGRendering
{
	public static SvgDocument RenderSVGFromStack(List<StackItem> stack)
	{
		SvgDocument doc = new SvgDocument();
		SvgDocument temp = new SvgDocument();
		
		var rootCollection = temp.Children;
		stack.Reverse();
		foreach (var item in stack)
		{
			if (item is Shape shape)
			{
				shape.RenderToSVGParent(ref rootCollection);
			}
		}

		//
		foreach (var element in rootCollection)
		{
			doc.Children.Add(element);
		}

		return doc;
	}

	public static SvgElement RenderToSVGElement(this Polygon polygon)
	{
		if (polygon.NumInteriorRings == 0)
		{
			return polygon.Shell.RenderToSVGElement();

		}
		else
		{
			throw new NotImplementedException("polygons with interior rings not yet supported");
			//render each interior ring
			//create subtraction or addition group depending n CW/CCW (i think?)
			//return that group or boolean thing.
		}
	}

	public static SvgPolygon RenderToSVGElement(this LinearRing ring)
	{
		Console.WriteLine($"Rendering LinearRing of type {ring.GeometryType}");

		var list = new SvgPointCollection();
		for (int os = 0; os < ring.NumPoints; os++)
		{
			var c = ring.GetCoordinateN(os);
			list.Add(new SvgUnit(SvgUnitType.None, (float)c.X));
			list.Add(new SvgUnit(SvgUnitType.None, (float)c.Y));
		}

		var polygon = new SvgPolygon()
		{
			Points =list
		};
		return polygon;
	}
}