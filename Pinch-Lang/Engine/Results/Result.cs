using Svg;

namespace Pinch_Lang.Engine;

public class Result
{
	public SvgDocument Document;
	public bool DidSucceed;
	public List<PinchWarning>? Warnings;
	public Exception? Error;

	public static Result Success(SvgDocument doc)
	{
		return new Result()
		{
			Document = doc,
			DidSucceed = true,
			Warnings = null,
			Error = null
		};
	}

	private static SvgDocument EmptySvgDoc = new SvgDocument();
	
	public static Result GetErrorResult(Exception e)
	{
		return new Result()
		{
			Document = EmptySvgDoc,
			DidSucceed = false,
			Warnings = null,
			Error = e
		};
	}
}

public struct PinchWarning
{
	public string Message;

	public PinchWarning(string message)
	{
		Message = message;
	}
}
