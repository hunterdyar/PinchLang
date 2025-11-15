using System.Text;
using System.Xml;
using Pinch_Lang.Engine;
using ShapesDeclare;
using ShapesDeclare.AST;
using Svg;
using Environment = Pinch_Lang.Engine.Environment;

namespace PinchLangTests;

public class Tests
{
	[SetUp]
	public void Setup()
	{
		
	}

	private void WriteFile(SvgDocument svg)
	{
		var _svgBuilder = new StringBuilder();
		var writer = XmlWriter.Create(_svgBuilder,
			new XmlWriterSettings { Indent = true, ConformanceLevel = ConformanceLevel.Fragment });
		svg.Width = new SvgUnit(SvgUnitType.Percentage, 100);
		svg.Height = new SvgUnit(SvgUnitType.Percentage, 100);
		svg.Write(writer);
		writer.Flush();
		var svgData = _svgBuilder.ToString();
		var html =
			$"<html><body style=\"background-color: green;\"><p>({DateTime.Now}) svg:</p>\n<div style=\"display: block; background-color: lightblue;\"> {svgData}</div><p>//svg</p>\n</body></html>";

		string docPath = Path.Combine(System.Environment.CurrentDirectory, "test.html");
		using (StreamWriter outputFile = new StreamWriter(docPath))
		{
				outputFile.Write(html);
		}
	}


	[Test]
	public void ShapesNoPushPop()
	{
		var i = """
		        [shapes]
		        rect 0 0 10 10
		        rect 5 5 20 20
		        """;
		var p = ShapeParser.TryParse(i, out Root root, out var error);
		if (!p)
		{
			Assert.Fail(error);
		}

		var e = new Environment();
		var svg = e.Execute(root);
		WriteFile(svg);
	}

	

	[Test]
	public void WalkerSVG()
	{
		var i = """
		        [shapes]
		        rect 0 0 10 20
		        rect 10 10 20 20
		        
		        """;
		var p = ShapeParser.TryParse(i, out Root root, out var error);
		if (!p)
		{
			Assert.Fail();
		}

		var e = new Environment();

		var svg = e.Execute(root);
		Assert.That(svg.Children.Count == 2);
	}
}