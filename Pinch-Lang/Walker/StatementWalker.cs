using NetTopologySuite.Geometries;
using Pinch_Lang.Engine;
using ShapesDeclare.AST;
using Environment = Pinch_Lang.Engine.Environment;
namespace Pinch_Lang.Walker;

public class StatementWalker
{
	private readonly Environment _environment;

	public StatementWalker(Environment environment)
	{
		_environment = environment;
	}

	public void Walk(Root root)
	{
		//why not be allowed to declare a [shapes] then a [metadata] then a [shapes] again? just treat them as in one order....

		foreach (var sections in root.Sections)
		{
			if (sections.Header.Title == "shapes")
			{
				foreach (var statement in sections.Statements)
				{
					WalkStatement(statement);
				}
			} //else for properties, for headers, etc.
		}
	}

	private void WalkStatement(Statement statement)
	{
		switch (statement)
		{
			case ShapeDeclaration shapeDeclaration:
				WalkShapeDeclare(shapeDeclaration);
				break;
			case FunctionCall functionCall:
				WalkFunctionCall(functionCall);
				break;
			case Push push:
				var item = WalkShapeDeclare(push.Shape);
				_environment.Push(item);
				break;
			case Pop pop:
				_environment.Pop();
				break;
		}
	}
	
	private StackItem WalkShapeDeclare(ShapeDeclaration shapeDeclaration)
	{
		var shapeName = shapeDeclaration.Name.ToString();
		var shapeType = shapeDeclaration.ShapeType.ToString();
		var args = shapeDeclaration.Expressions;
		if (shapeType == "circle")
		{
			// var c = new CGALDotNetGeometry.Shapes.Circle2f(0,0,0);
			// var shape = new Circle(_environment, c);
			// var args = shapeDeclaration.Expressions;
			// if (args.Length == 0)
			// {
			// 	//all good!
			// }else if (args.Length == 1)
			// {
			// 	//radius
			// 	shape.SetProperty("radius", args[0]);
			// }else if (args.Length == 2)
			// {
			// 	//todo: support for vec2's
			// 	shape.SetProperty("center_x", args[0]);
			// 	shape.SetProperty("center_y", args[1]);
			// }else if (args.Length == 3)
			// {
			// 	shape.SetProperty("center_x", args[0]);
			// 	shape.SetProperty("center_y", args[1]);
			// 	shape.SetProperty("radius", args[2]);
			// }
			// else
			// {
			// 	throw new Exception("Invalid number of arguments for Circle. Expected 1 (r), 2(x,y) or 3(x,y,r)");
			// }
			//
			// _environment.DeclareShape(shapeName, shape);
			// return shape;
		}else if (shapeType == "rect")
		{
			if (args.Length == 4)
			{
				var min_x = ValueItem.AsNumber(_environment.ExprWalker.WalkExpression(args[0]));
				var min_y = ValueItem.AsNumber(_environment.ExprWalker.WalkExpression(args[1]));
				var max_x = ValueItem.AsNumber(_environment.ExprWalker.WalkExpression(args[2]));
				var max_y = ValueItem.AsNumber(_environment.ExprWalker.WalkExpression(args[3]));
				var rect = new Rect(_environment, new Coordinate(min_x, min_y), new Coordinate(max_x, max_y));
				return rect;
			}
		}

		throw new Exception($"Invalid Shape Type {shapeType} (for shape {shapeName})");
	}

	private void WalkFunctionCall(FunctionCall functionCall)
	{
		switch (functionCall.Name.ToString())
		{
			case "set":
				Builtins.Set(_environment, functionCall.Arguments);
				break;
			default:
				throw new Exception($"Unknown function or module '{functionCall.Name}'");
		}
	}
}