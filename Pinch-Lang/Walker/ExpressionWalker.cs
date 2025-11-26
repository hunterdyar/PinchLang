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
				if (_environment.CurrentFrame.TryGetValueItem(id.Value.ToString(), out var vi))
				{
					return vi;
				}
				else
				{
					throw new Exception($"Unkown variable {id}");
				}

				return new IdentifierValue(id.Value);
			case BinaryOperator binOp:
				return binOp.Evaluate(_environment);
			case UnaryOperator unOp:
				return WalkUnary(unOp);
			case FunctionExpressionCall fc:
				return WalkFunctionCall(fc);
		}

		throw new NotImplementedException($"unable to walk {expression}");
	}

	private ValueItem WalkFunctionCall(FunctionExpressionCall fc)
	{
		var callName = fc.Name.ToString();
		
		if (BuiltinFunctions.Builtins.TryGetValue(callName, out var func))
		{
			//walk arguments
			ValueItem[] args = new ValueItem[fc.Arguments.Length];
			for (int i = 0; i < args.Length; i++)
			{
				args[i] = _environment.ExprWalker.WalkExpression(fc.Arguments[i]);
			}
			return func.Invoke(_environment, args);
		}
		else
		{
			throw new Exception($"Unknown Function Call {callName}");
		}
	}

	private ValueItem WalkUnary(UnaryOperator op)
	{
		var left = WalkExpression(op.Operand);
		switch (op)
		{
			case Negate negate:
				//if it's already a number, prevent an extra allocation.
				if (left is NumberValue nv)
				{
					nv.Value = -nv.Value;
					return nv;
				}
				//otherwise, attempt to cast it. 
				return new NumberValue(-left.AsNumber());
			default:
				throw new NotImplementedException($"Unknown Unary Operator {op}");
		}
	}
	private ValueItem WalkBinary(BinaryOperator bin)
	{
		return bin.Evaluate(_environment);
	}
}