using Pinch_Lang.Engine;
using ShapesDeclare.AST;
using Environment = Pinch_Lang.Engine.Environment;

namespace Pinch_Lang.Walker;

public class ExpressionWalker
{
	//get values from items!
	private Environment _environment;

	public ExpressionWalker(Environment environment)
	{
		_environment = environment;
	}

	public ValueItem WalkExpression(Expression expression)
	{
		switch (expression)
		{
			case NumberLiteral numberLiteral:
				return new NumberValue(numberLiteral.Value);
			case StringLiteral stringLiteral:
				return new StringValue(stringLiteral.Value);
			case Identifier id:
				return new IdentifierValue(id.Value);
			case BinaryOperator binOp:
				throw new NotImplementedException("binopop");
		}

		throw new NotImplementedException($"unable to walk {expression}");
	}
}