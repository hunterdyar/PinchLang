using System.Text;
using System.Xml;
using Pinch_Lang.Engine;
using Svg;
using Environment = Pinch_Lang.Engine.Environment;

namespace ShapesDeclare.Utility;

public static class EnvUtil
{
	public static StackItem Top(this Environment env)
	{
		return env.CurrentFrame.TopStackItem();
	}

	public static string SvgDocumentToString(SvgDocument svg)
	{
		var _svgBuilder = new StringBuilder();
		var writer = XmlWriter.Create(_svgBuilder,
			new XmlWriterSettings { Indent = true, ConformanceLevel = ConformanceLevel.Fragment });
		svg.Write(writer);
		writer.Flush();
		var svgData = _svgBuilder.ToString();
		return svgData;
	}
}