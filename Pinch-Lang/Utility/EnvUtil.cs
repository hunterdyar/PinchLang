using Pinch_Lang.Engine;
using Environment = Pinch_Lang.Engine.Environment;

namespace ShapesDeclare.Utility;

public static class EnvUtil
{
	public static StackItem Top(this Environment env)
	{
		return env.CurrentFrame.TopStackItem();
	}
}