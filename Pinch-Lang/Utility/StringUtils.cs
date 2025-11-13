using System.Text;

namespace ShapesDeclare.Utility;

public static class StringUtils
{
	private static StringBuilder _tsdelimSB = new StringBuilder();
	public static string ToStringDelimited(this object[] list, string sep)
	{
		_tsdelimSB.Clear();

		if (list.Length == 0)
		{
			return "";
		}
		
		for (int i = 0; i < list.Length; i++)
		{
			_tsdelimSB.Append(list[i].ToString());
			if (i < list.Length - 1)
			{
				_tsdelimSB.Append(sep);
			}
		}

		return _tsdelimSB.ToString();
	}
}