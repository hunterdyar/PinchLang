using Svg;

namespace Pinch_Lang.Engine;

public class Result
{
	public SvgDocument Document;
	public bool DidSucceed;
	public List<ResultMessage>? Messages;
	public Exception? Error;

	public static Result Success(SvgDocument doc)
	{
		return new Result()
		{
			Document = doc,
			DidSucceed = true,
			Messages = null,
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
			Messages = new List<ResultMessage>()
			{
				new ResultMessage(ResultMessageType.Error, e.Message)
			},
			Error = e
		};
	}
}
