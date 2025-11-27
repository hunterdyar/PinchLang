using Superpower.Model;

namespace ShapesDeclare.AST;

public enum UnOp
{
    Negate,
}

public class UnaryOperator : Operation
{
    public Expression Operand => _operand;
    protected Expression _operand;
    
    public UnaryOperator(Expression exp)
    {
        _operand = exp;
    }
    
}

public class Negate : UnaryOperator
{
    public Negate(Expression exp) : base(exp)
    {
    }

    public override string ToString()
    {
        return "(-" + _operand.ToString()+")";
    }
}