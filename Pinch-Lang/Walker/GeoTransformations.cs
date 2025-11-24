using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Utilities;
using Pinch_Lang.Engine;
using Environment = Pinch_Lang.Engine.Environment;
namespace Pinch_Lang.Walker;

public static class GeoTransformations
{
	

	public static void Translate(Environment env, ValueItem[] args, List<StackItem> items)
	{
		Builtins.ValidateArgumentCount("translate", args.Length, [["delta x", "delta y"]]);
		var dx = args[0].AsNumber();
		var dy = args[1].AsNumber();

		if (items.Count > 0)
		{
			//rotate each item and put it on the stack
			throw new NotImplementedException();
		}
		else
		{
			var item = env.CurrentFrame.TopStackItem();
			var shape = item as Shape;
			var gt = AffineTransformation.TranslationInstance(dx,dy);
			shape.AffineTransform(gt);
		}
	}

	public static void TranslateX(Environment env, ValueItem[] args, List<StackItem> items)
	{
		Builtins.ValidateArgumentCount("tx", args.Length, [["delta x"]]);
		var dx = args[0].AsNumber();

		if (items.Count > 0)
		{
			throw new NotImplementedException();
		}
		else
		{
			var item = env.CurrentFrame.TopStackItem();
			var shape = item as Shape;
			var gt = AffineTransformation.TranslationInstance(dx, 0);
			shape.AffineTransform(gt);
		}
	}

	public static void TranslateY(Environment env, ValueItem[] args, List<StackItem> items)
	{
		Builtins.ValidateArgumentCount("ty", args.Length, [["delta y"]]);
		var dy = args[0].AsNumber();

		if (items.Count > 0)
		{
			throw new NotImplementedException();
		}
		else
		{
			var item = env.CurrentFrame.TopStackItem();
			var shape = item as Shape;
			var gt = AffineTransformation.TranslationInstance(0, dy);
			shape.AffineTransform(gt);
		}
	}

	/// <summary>
	/// Rotate a shape around it's centroid
	/// </summary>
	public static void Rotate(Environment env, ValueItem[] args, List<StackItem> items)
	{
		Builtins.ValidateArgumentCount("rotate", args.Length, [["angle"]]);
		var angle = env.AngleToRadians(args[0].AsNumber());

		if (items.Count > 0)
		{
			throw new NotImplementedException();
		}
		else
		{
			var item = env.CurrentFrame.TopStackItem();
			var shape = item as Shape;
			var g = shape?.GetGeometry();
			var center = g?.Centroid;
			if (center == null)
			{
				throw new Exception("stack item unable to be coerced into shape.");
			}

			var gt = AffineTransformation.RotationInstance(angle, center.X, center.Y);
			shape.AffineTransform(gt);
		}
	}

	public static void RotateAround(Environment env, ValueItem[] args, List<StackItem> items)
	{
		Builtins.ValidateArgumentCount("rotate around", args.Length, [["angle", "around x", "around y"]]);
		var angle = env.AngleToRadians(args[0].AsNumber());
		var rx = env.AngleToRadians(args[1].AsNumber());
		var ry = env.AngleToRadians(args[2].AsNumber());
		
		if (items.Count > 0)
		{
			throw new NotImplementedException();
		}
		else
		{
			var item = env.CurrentFrame.TopStackItem();
			var shape = item as Shape;
			//var g = shape?.GetGeometry();
			//var center = g?.Centroid;
			// if (center == null)
			// {
			// 	throw new Exception("stack item unable to be coerced into shape.");
			// }

			var gt = AffineTransformation.RotationInstance(angle, rx, ry);
			shape.AffineTransform(gt);
		}
	}

	/// <summary>
	/// Rotates a shape around a point dx/dy units away from it's centroid position
	/// </summary>
	public static void RotateAroundRelative(Environment env, ValueItem[] args, List<StackItem> items)
	{
		Builtins.ValidateArgumentCount("rotate around relative", args.Length, [["angle", "around x (rel)", "around y (rel)"]]);
		var angle = env.AngleToRadians(args[0].AsNumber());
		var rx = env.AngleToRadians(args[1].AsNumber());
		var ry = env.AngleToRadians(args[2].AsNumber());

		if (items.Count > 0)
		{
			throw new NotImplementedException();
		}
		else
		{
			var item = env.CurrentFrame.TopStackItem();
			var shape = item as Shape;
			var g = shape?.GetGeometry();
			var center = g?.Centroid;
			 if (center == null)
			 {
			 	throw new Exception("stack item unable to be coerced into shape.");
			 }

			var gt = AffineTransformation.RotationInstance(angle, center.X+rx, center.Y+ry);
			shape.AffineTransform(gt);
		}
	}

	public static void Scale(Environment env, ValueItem[] args, List<StackItem> items)
	{
		Builtins.ValidateArgumentCount("scale", args.Length, [["scale"],["scale x", "scale y"]]);
		var scaleX = args[0].AsNumber();
		var scaleY = scaleX;
		if (args.Length == 2)
		{
			scaleY = args[1].AsNumber();
		}
		
		if (items.Count > 0)
		{
			throw new NotImplementedException();
		}
		else
		{
			var item = env.CurrentFrame.TopStackItem();
			var shape = item as Shape;
			var geoCenter = shape.GetGeometry().Centroid ?? throw new Exception("can't coerce stackItem into shape for calculating centroid to scale around.");
			var gt = AffineTransformation.ScaleInstance(scaleX, scaleY, geoCenter.X, geoCenter.Y);
			shape.AffineTransform(gt);
		}
	}

	public static void ScaleAround(Environment env, ValueItem[] args, List<StackItem> items)
	{
		Builtins.ValidateArgumentCount("scale_around", args.Length, [["scale", "around x", "around y"], ["scale x", "scale y", "around x", "around y"]
		]);
		int j = 0;
		var scaleX = args[j].AsNumber();
		var scaleY = scaleX;
		
		if (args.Length == 4)
		{
			j++;
			scaleY = args[j].AsNumber();
		}//else scaleY is X.

		j++;
		double aroundX = args[j].AsNumber();
		j++;
		double aroundY = args[j].AsNumber();
		
		if (items.Count > 0)
		{
			throw new NotImplementedException();
		}
		else
		{
			var item = env.CurrentFrame.TopStackItem();
			var shape = item as Shape;
			var gt = AffineTransformation.ScaleInstance(scaleX, scaleY, aroundX, aroundY);
			shape.AffineTransform(gt);
		}
	}

	public static void ScaleAroundRelative(Environment env, ValueItem[] args, List<StackItem> items)
	{
		Builtins.ValidateArgumentCount("scale_around_relative", args.Length, [["scale", "around x (rel)", "around y (rel)"], ["scale x", "scale y", "around x (rel)", "around y (rel)"]
		]);
		int j = 0;
		var scaleX = args[j].AsNumber();
		var scaleY = scaleX;

		if (args.Length == 4)
		{
			j++;
			scaleY = args[j].AsNumber();
		} //else scaleY is X.

		j++;
		double aroundXrel = args[j].AsNumber();
		j++;
		double aroundYrel = args[j].AsNumber();

		if (items.Count > 0)
		{
			throw new NotImplementedException();
		}
		else
		{
			var item = env.CurrentFrame.TopStackItem();
			var shape = item as Shape;
			var geoCenter = shape.GetGeometry().Centroid ??
			                throw new Exception(
				                "can't coerce stackItem into shape for calculating centroid to scale around relative.");
			var gt = AffineTransformation.ScaleInstance(scaleX, scaleY, geoCenter.X+aroundXrel, geoCenter.Y+aroundYrel);
			shape.AffineTransform(gt);
		}
	}
}