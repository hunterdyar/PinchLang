using Pinch_Lang.Engine;
using Environment = Pinch_Lang.Engine.Environment;

namespace ShapesDeclare.Utility;

public static class EnvUtil
{
	public static StackItem Top(this Environment env)
	{
		if (env.ShapeStack.Count == 0)
		{
			throw new Exception("Stack Empty!");
		}

		return env.ShapeStack[^1];
	}
}