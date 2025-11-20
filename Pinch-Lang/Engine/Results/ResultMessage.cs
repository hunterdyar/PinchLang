namespace Pinch_Lang.Engine;

public enum ResultMessageType
{
	Notice,
	Warning,
	Error
}
public struct ResultMessage
{
	public DateTime DateTime { get; }
	public ResultMessageType Type { get; }
	public string Message { get; }

	public ResultMessage(ResultMessageType type, string message)
	{
		DateTime = DateTime.Now;
		Type = type;
		Message = message;
	}
}