using CGALDotNetGeometry.Shapes;
using Pinch_Lang.Engine;
using ShapesDeclare;
using ShapesDeclare.AST;
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
		        """;
		var p = ShapeParser.TryParse(i, out Root root, out var error);
		if (!p)
		{
			Assert.Fail();
		}
		var e = new Environment();
		
		e.Execute(root);
		
		Assert.That(e.Declarations["c1"] != null);
		var c = (Shape2D<Circle2f>)e.Declarations["c1"];
		Assert.That(ValueItem.AsNumber(c.Properties["radius"]) == 20);
	}
}