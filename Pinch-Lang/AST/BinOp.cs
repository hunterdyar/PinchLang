using Pinch_Lang.Engine;
using Pinch_Lang.Walker;
using Environment = Pinch_Lang.Engine.Environment;

namespace ShapesDeclare.AST;

public enum BinOp
{
	Plus,
	Minus,
	Times,
	Divide,
	Modulo,
	Pow,
}

public class OpItem
{
	public BinOp Op => _op;
	private BinOp _op;
	public OpItem(BinOp op)
	{
		_op = op;
	}
}
public class Operation : Expression
{
	
}


public abstract class BinaryOperator : Operation
{
	public Expression Left;
	public Expression Right;

	public BinaryOperator(Expression left, Expression right)
	{
		this.Left = left;
		this.Right = right;
	}
	public abstract ValueItem Evaluate(Environment env);

}

public class Plus : BinaryOperator
{
	public Plus(Expression left, Expression right) : base(left, right)
	{
	}

	public override ValueItem Evaluate(Environment env)
	{
		var lv = env.ExprWalker.WalkExpression(Left);
		var rv = env.ExprWalker.WalkExpression(Right);

		if (lv is NumberValue ln && rv is NumberValue rn)
		{
			return new NumberValue(ln.Value+rn.Value);
		}
		
		if (lv is StringValue)
		{
			return new StringValue(lv.AsStringOrID()+rv.AsStringOrID());
		}
		
		if (rv is StringValue)
		{
			return new StringValue(lv.AsStringOrID()+rv.AsStringOrID());
		}

		throw new Exception("Unable to evaluate plus operator, invalid operand types.");
	}
}
public class Minus : BinaryOperator
{
	public Minus(Expression left, Expression right) : base(left, right)
	{
	}
	public override ValueItem Evaluate(Environment env)
	{
		var lv = env.ExprWalker.WalkExpression(Left);
		var rv = env.ExprWalker.WalkExpression(Right);

		if (lv is NumberValue ln && rv is NumberValue rn)
		{
			return new NumberValue(ln.Value - rn.Value);
		}
		
		throw new Exception("Unable to evaluate plus operator, invalid operand types.");
	}
}

public class Multiply : BinaryOperator
{
	public Multiply(Expression left, Expression right) : base(left, right)
	{
	}
	
	public override ValueItem Evaluate(Environment env)
	{
		var lv = env.ExprWalker.WalkExpression(Left);
		var rv = env.ExprWalker.WalkExpression(Right);

		if (lv is NumberValue ln && rv is NumberValue rn)
		{
			return new NumberValue(ln.Value * rn.Value);
		}
		
		throw new Exception("Unable to evaluate plus operator, invalid operand types.");
	}
}

public class Divide : BinaryOperator
{
	public Divide(Expression left, Expression right) : base(left, right)
	{
	}
	public override ValueItem Evaluate(Environment env)
	{
		var lv = env.ExprWalker.WalkExpression(Left);
		var rv = env.ExprWalker.WalkExpression(Right);

		if (lv is NumberValue ln && rv is NumberValue rn)
		{
			return new NumberValue(ln.Value / rn.Value);
		}
		
		throw new Exception("Unable to evaluate plus operator, invalid operand types.");
	}
}

public class Modulo : BinaryOperator
{
	public Modulo(Expression left, Expression right) : base(left, right)
	{
	}
	public override ValueItem Evaluate(Environment env)
	{
		var lv = env.ExprWalker.WalkExpression(Left);
		var rv = env.ExprWalker.WalkExpression(Right);

		if (lv is NumberValue ln && rv is NumberValue rn)
		{
			return new NumberValue(ln.Value % rn.Value);
		}
		
		throw new Exception("Unable to evaluate plus operator, invalid operand types.");
	}
}

