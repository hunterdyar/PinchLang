using System.Text;
using NetTopologySuite.Geometries;
using Pinch_Lang.Engine;
using ShapesDeclare.Utility;
using  Environment = Pinch_Lang.Engine.Environment;
namespace Pinch_Lang.Walker;

public static class Builtins
{
	public static readonly Dictionary<string, Action<Environment, ValueItem[], List<StackItem>>> BuiltinLookup = new Dictionary<string, Action<Environment, ValueItem[], List<StackItem>>>()
	{
		//general
		{ "set", Set},
		{ "group", Group },
		{ "canvas", Canvas},

		//shape prims
		{ "circle", Circle},
		{ "rect", Rect},
		{ "crect", CenterRect},
		{ "mmrect", MinMaxRect},

		//GeoProcessing.cs functions
		{ "convex_hull", Walker.GeoProcessing.ConvexHull},
		{ "difference", GeoProcessing.Difference},
		{ "intersect", GeoProcessing.Intersect },
		{ "buffer", GeoProcessing.Buffer },
		{ "instance", GeoProcessing.Instance },
		{ "array", GeoProcessing.Array },

		//GeoTransformations.cs functions
		{ "translate", GeoTransformations.Translate },
		{ "t", GeoTransformations.Translate },
		{ "tx", GeoTransformations.TranslateX },
		{ "ty", GeoTransformations.TranslateY },
		
		{ "rotate", GeoTransformations.Rotate },
		{ "r", GeoTransformations.Rotate },
		{ "rotate_around", GeoTransformations.RotateAround },
		{ "ra", GeoTransformations.RotateAround },
		{ "rotate_around_relative", GeoTransformations.RotateAroundRelative },
		{ "rar", GeoTransformations.RotateAroundRelative },
		
		{ "scale", GeoTransformations.Scale },
		{ "s", GeoTransformations.Scale },
		{ "scale_around", GeoTransformations.ScaleAround },
		{ "sa", GeoTransformations.ScaleAround },
		{ "scale_around_rel", GeoTransformations.ScaleAroundRelative },
		{ "sar", GeoTransformations.ScaleAroundRelative },
	};

	public static bool ValidateArgumentCount(string funcName, int providedCount, string[][] signatures)
	{
		var valid = signatures.Any(x => x.Length == providedCount);
		if (!valid)
		{
			StringBuilder err = new StringBuilder();
			err.Append($"Bad number of arguments for {funcName}. Expected ");
			if (signatures.Length == 0)
			{
				err.Append('0');
			}
			else
			{
				for (var i = 0; i < signatures.Length; i++)
				{
					var sig = signatures[i];
					err.Append(sig.Length);
					err.Append(' ');
					if (sig.Length > 0)
					{
						err.Append('(');
						for (int j = 0; j < sig.Length; j++)
						{
							err.Append(sig[j]);
							if (j < sig.Length - 1)
							{
								err.Append(' ');
							}
						}

						err.Append(')');
					}

					if (i < signatures.Length - 1)
					{
						err.Append(" or ");
					}
				}
			}

			err.Append(". Got ");
			err.Append(providedCount);
			err.Append('.');
			throw new Exception(err.ToString());
		}

		return valid;
	}

	#region General

	public static void Set(Environment env, ValueItem[] args, List<StackItem> context)
	{
		ValidateArgumentCount("set", args.Length, [["property", "value"]]);
		var propNameItem = args[0];
		var propName = propNameItem.AsStringOrID();
		
		//get top of shapestack
		var si = env.Top();
		
		//call SetProperty on it with PropName.
		si.SetProperty(propName, args[1]);
	}

	public static void Canvas(Environment env, ValueItem[] args, List<StackItem> context)
	{
		ValidateArgumentCount("set", args.Length, []);
		env.AddToCanvas(context);//clone?
	}

	public static void Group(Environment env, ValueItem[] args, List<StackItem> context)
	{
		ValidateArgumentCount("group", args.Length, [[],["Num To Pop"]]);
		if (args.Length == 1)
		{
			context.Add(env.CurrentFrame.PopStackItem());
		}

		var g = new PolyGroup(env, context);
		env.Push(g);
	}
	
	

	#endregion

	#region Shape Primitives
	
	public static void Circle(Environment env, ValueItem[] args, List<StackItem> context)
	{
		ValidateArgumentCount("circle", args.Length, [
			["radius"],
			["centerX", "centerY", "radius"]
		]);
		double cx=0, cy=0, radius = 0;
		
		if (args.Length == 1)
		{
			radius = args[0].AsNumber();
		}
		else if (args.Length == 3)
		{
			cx = args[0].AsNumber();
			cy = args[1].AsNumber();
			radius = args[2].AsNumber();
		}else
		{
			//this shouldn't happen because of ValidateArgumentCount
			throw new Exception("Error, method count not correctly validated.");
		}

		var circle = new Circle(env, new Coordinate(cx, cy), radius);
		env.Push(circle);
	}
	
	public static void MinMaxRect(Environment env, ValueItem[] args, List<StackItem> context)
	{
		ValidateArgumentCount("rect", args.Length, [
			["minX", "minY", "maxX", "maxY"],
		]);
		double minx = args[0].AsNumber();
		double minY = args[1].AsNumber();
		double maxX = args[2].AsNumber();
		double maxY = args[3].AsNumber();

		var rect = new Rect(env, new Coordinate(minx, minY), new Coordinate(maxX, maxY));
		env.Push(rect);
	}

	public static void Rect(Environment env, ValueItem[] args, List<StackItem> context)
	{
		ValidateArgumentCount("rect", args.Length, [
			["minX", "minY", "width", "height"],
		]);
		double minx = args[0].AsNumber();
		double minY = args[1].AsNumber();
		double width = args[2].AsNumber();
		double height = args[3].AsNumber();

		if (width < 0)
		{
			minx -= width;
			width = Math.Abs(width);
		}
		if (height < 0)
		{
			minY -= height;
			height = Math.Abs(height);
		}
		
		var rect = new Rect(env, new Coordinate(minx, minY), new Coordinate(minx+width, minY+height));
		env.Push(rect);
	}
	
	public static void CenterRect(Environment env, ValueItem[] args, List<StackItem> context)
	{
		ValidateArgumentCount("center_rect", args.Length, [
			["minX", "minY", "width", "height"],
		]);
		double cx = args[0].AsNumber();
		double cy = args[1].AsNumber();
		double width = args[2].AsNumber();
		double height = args[3].AsNumber();

		if (width < 0)
		{
			width = Math.Abs(width);
		}
		if (height < 0)
		{
			height = Math.Abs(height);
		}

		width = width / 2;
		height = height / 2;
		
		var rect = new Rect(env, new Coordinate(cx-width,cy-height), new Coordinate(cx+width, cx+height));
		env.Push(rect);
	}

	#endregion

	
}