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
				return binOp.Evaluate(_environment);
			case UnaryOperator unOp:
				return WalkUnary(unOp);
		}

		throw new NotImplementedException($"unable to walk {expression}");
	}

	private ValueItem WalkUnary(UnaryOperator op)
	{
		var left = WalkExpression(op.Operand);
		switch (op.Op)
		{
			case UnOp.Negate:
				//if it's already a number, prevent an extra allocation.
				if (left is NumberValue nv)
				{
					nv.Value = -nv.Value;
					return nv;
				}
				//otherwise, attempt to cast it. 
				return new NumberValue(-left.AsNumber());
			default:
				throw new NotImplementedException($"Unknown Unary Operator {op.Op}");
		}
	}
	private ValueItem WalkBinary(BinaryOperator bin)
	{
		return bin.Evaluate(_environment);
	}
}