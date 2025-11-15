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
				var x = WalkShapeDeclare(shapeDeclaration);
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
			var cx = _environment.ExprWalker.WalkExpression(args[0]).AsNumber();
			var cy = _environment.ExprWalker.WalkExpression(args[1]).AsNumber();
			var radius = _environment.ExprWalker.WalkExpression(args[2]).AsNumber();
			var circle = new Circle(_environment, new Coordinate(cx, cy), radius);
			//env.DeclareShape...
			_environment.Push(circle);
			return circle;
		}else if (shapeType == "rect")
		{
			if (args.Length == 4)
			{
				var min_x = _environment.ExprWalker.WalkExpression(args[0]).AsNumber();
				var min_y = _environment.ExprWalker.WalkExpression(args[1]).AsNumber();
				var max_x = _environment.ExprWalker.WalkExpression(args[2]).AsNumber();
				var max_y = _environment.ExprWalker.WalkExpression(args[3]).AsNumber();
				var rect = new Rect(_environment, new Coordinate(min_x, min_y), new Coordinate(max_x, max_y));
				// _environment.DeclareShape(shapeName, rect);
				_environment.Push(rect);
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