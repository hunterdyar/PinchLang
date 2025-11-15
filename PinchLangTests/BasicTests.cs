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

	[Test]
	public void Test1()
	{
		var i = """
		        [shapes]
		        c1:circle >
		        .
		        """;
		var p = ShapeParser.TryParse(i
			, out Root root, out var error);
		Assert.That(p, Is.True);
		Assert.That(root.ToString().Trim(), Is.EqualTo(i.Trim()));
	}

	[Test]
	public void Walker1()
	{
		var i = """
		        [shapes]
		        c1:circle 0 0 10 >
		        set radius 20
		        .
		        r1:rect 5 5 6 6
		        """;
		var p = ShapeParser.TryParse(i, out Root root, out var error);
		if (!p)
		{
			Assert.Fail();
		}
		var e = new Environment();
		
		e.Execute(root);
		
		// Assert.That(ValueItem.AsNumber(c.Properties["radius"]) == 20);
	}
	

	[Test]
	public void ShapesNoPushPop()
	{
		var i = """
		        [shapes]
		        r1:rect 0 0 10 10
		        r2:rect 5 5 20 20
		        """;
		var p = ShapeParser.TryParse(i, out Root root, out var error);
		if (!p)
		{
			Assert.Fail(error);
		}

		var e = new Environment();
		e.Execute(root);
	}


	[Test]
	public void WalkerSVG()
	{
		var i = """
		        [shapes]
		        r1:rect 0 0 10 10 >
		        .
		        
		        r2:rect 10 10 20 20 > 
		        .
		        """;
		var p = ShapeParser.TryParse(i, out Root root, out var error);
		if (!p)
		{
			Assert.Fail();
		}

		var e = new Environment();

		e.Execute(root);
		var svg = e.RenderSVG();
		Assert.That(svg.Children.Count == 2);
	}
}