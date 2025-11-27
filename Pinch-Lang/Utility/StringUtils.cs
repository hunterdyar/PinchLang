using System.Text;

namespace ShapesDeclare.Utility;

public static class StringUtils
{
	public static string ToStringDelimited(this object[] list, string sep)
	{
		var tsdelimSB = new StringBuilder();

		if (list.Length == 0)
		{
			return "";
		}
		
		for (int i = 0; i < list.Length; i++)
		{
			tsdelimSB.Append(list[i].ToString());
			if (i < list.Length - 1)
			{
				tsdelimSB.Append(sep);
			}
		}

		return tsdelimSB.ToString();
	}
}