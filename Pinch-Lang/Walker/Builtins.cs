using Pinch_Lang.Engine;
using ShapesDeclare.AST;
using ShapesDeclare.Utility;
using  Environment = Pinch_Lang.Engine.Environment;
namespace Pinch_Lang.Walker;

public static class Builtins
{
	public static Dictionary<string, Action<Environment, ValueItem[], List<StackItem>>> BuiltinLookup = new Dictionary<string, Action<Environment, ValueItem[], List<StackItem>>>()
	{
		{ "set", Set},
	};
	public static void Set(Environment env, ValueItem[] args, List<StackItem> context)
	{
		var propNameItem = args[0];
		var propName = propNameItem.AsStringOrID();
		
		//get top of shapestack
		var si = env.Top();
		
		//call SetProperty on it with PropName.
		si.SetProperty(propName, args[1]);
	}
}