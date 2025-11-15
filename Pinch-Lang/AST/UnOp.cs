using Superpower.Model;

namespace ShapesDeclare.AST;

public enum UnOp
{
    Negate,
}



public class UnaryOperator(UnOp op, Expression exp) : Operator
{
    public readonly UnOp Op = op;
    private Expression _operand = exp;
    
    public static UnaryOperator CreateUnary(Token<SToken> op, Expression exp)
    {
        switch (op.Kind)
        {
            case SToken.Minus:
                return new UnaryOperator(UnOp.Negate, exp);
            default:
                throw new Exception("Unknown Unary Operator: " + op.Kind+". ("+op.Span+")");
        }
    }
}