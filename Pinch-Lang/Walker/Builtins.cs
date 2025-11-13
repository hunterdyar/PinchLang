using Pinch_Lang.Engine;
using ShapesDeclare.AST;
using ShapesDeclare.Utility;
using  Environment = Pinch_Lang.Engine.Environment;
namespace Pinch_Lang.Walker;

public static class Builtins
{
	public static void Set(Environment env, Expression[] args)
	{
		var propNameItem = env.ExprWalker.WalkExpression(args[0]);
		var propName = ValueItem.AsStringOrID(propNameItem);
		
		//get top of shapestack
		var si = env.Top();
		
		//call SetProperty on it with PropName.
		si.SetProperty(propName, args[1]);
	}
}