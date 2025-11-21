using ExCSS;
using NetTopologySuite.Geometries;
using Svg;
using Svg.Pathing;
using Point = NetTopologySuite.Geometries.Point;

namespace Pinch_Lang.Engine;

public static class SVGRendering
{
	public static SvgDocument RenderSVGFromStack(CanvasProperties properties, List<StackItem> stack)
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

		doc.Fill = new SvgColourServer(System.Drawing.Color.DarkSlateGray);
		doc.Color = new SvgColourServer(System.Drawing.Color.Aquamarine);
		doc.Width = new SvgUnit(SvgUnitType.None, (float)properties.Width);
		doc.Height = new SvgUnit(SvgUnitType.None, (float)properties.Height);

		return doc;
	}

	public static SvgElement RenderToSVGElement(this Geometry geo)
	{
		if (geo is Polygon poly)
		{
			return poly.RenderPolyToElement();
		}else if (geo is GeometryCollection gc)
		{
			return gc.RenderGeoCollectionToElement();
		}else if (geo is LinearRing lr)
		{
			return lr.RenderLinearRingToElement();
		}else if (geo is Point point)
		{
			return point.RenderPointToElement();
		}
		else
		{
			throw new NotImplementedException($"dont yet have overload for {geo.GetType().ToString()}.");
		}
	}

	public static SvgElement RenderPolyToElement(this Polygon polygon)
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

	public static SvgPolygon RenderLinearRingToElement(this LinearRing ring)
	{
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

	public static SvgGroup RenderGeoCollectionToElement(this GeometryCollection gc)
	{
		var group = new SvgGroup();
		foreach (var geo in gc.Geometries)
		{
			var c = geo.RenderToSVGElement();
			group.Children.Add(c);
		}

		return group;
	}

	public static SvgCircle RenderPointToElement(this Point point)
	{
		Console.WriteLine($"Rendering geo of type {point.GeometryType}");
		var p = new SvgCircle();
		p.CenterX = new SvgUnit(SvgUnitType.None, (float)point.X);
		p.CenterY = new SvgUnit(SvgUnitType.None, (float)point.Y);
		p.Radius = new SvgUnit(SvgUnitType.Pixel, 0.5f);
		return p;
	}
}