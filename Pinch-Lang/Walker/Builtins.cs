using System.Text;
using NetTopologySuite.Geometries;
using Pinch_Lang.Engine;
using ShapesDeclare.AST;
using ShapesDeclare.Utility;
using  Environment = Pinch_Lang.Engine.Environment;
namespace Pinch_Lang.Walker;

public static class Builtins
{
	public static readonly Dictionary<string, Action<Environment, ValueItem[], List<StackItem>>> BuiltinLookup = new Dictionary<string, Action<Environment, ValueItem[], List<StackItem>>>()
	{
		//general
		{ "set", Set},
		
		//shape prims
		{ "circle", Circle},
		{ "rect", Rect},
		// { "rectangle", Rect },//we can overload function name synonyms if we want.

		//GeoProcessing.cs functions
		{ "convex_hull", Walker.GeoProcessing.ConvexHull},
		{ "difference", GeoProcessing.Difference},
	};

	public static bool ValidateArgumentCount(string funcName, int count, string[][] signatures)
	{
		var valid = signatures.Any(x => x.Length == count);
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
		}

		return valid;
	}

	#region General

	public static void Set(Environment env, ValueItem[] args, List<StackItem> context)
	{
		var propNameItem = args[0];
		var propName = propNameItem.AsStringOrID();
		
		//get top of shapestack
		var si = env.Top();
		
		//call SetProperty on it with PropName.
		si.SetProperty(propName, args[1]);
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

	public static void Rect(Environment env, ValueItem[] args, List<StackItem> context)
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

	#endregion

	
}