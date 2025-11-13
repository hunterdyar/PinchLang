using ShapesDeclare;
using Superpower;
using Superpower.Model;

namespace PinchLangTests;

public class ParserTests
{
	private TokenList<SToken> Tokenize(string input)
	{
		var tokr = ShapeTokenizer.Tokenizer.TryTokenize(input);

		if (!tokr.HasValue)
		{
			Assert.Fail(tokr.ToString());
		}

		return tokr.Value;
	}
	
	[Test]
	public void StandaloneDeclarationTest()
	{
		var i = """
		        c1:circle 0 0 10
		        
		        """;
		var t = Tokenize(i);
		var p = ShapeParser.StandaloneDeclaration.Invoke(t);
		if (!p.HasValue)
		{
			Assert.Fail(p.ToString());
		}
	}

	[Test]
	public void PushTest()
	{
		var i = """
		        rect_name:rect -5 -5 10 15 >
		        .
		        """;
		var t = Tokenize(i);
		var p = ShapeParser.Push.Invoke(t);
		if (!p.HasValue)
		{
			Assert.Fail(p.ToString());
		}
	}

	[Test]
	public void FunctionDecTest()
	{
		var i = """
		        translate -10 -10
		        set radius 20
		        round r1 20
		        
		        """;
		var t = Tokenize(i);
		var p = ShapeParser.FunctionCall.Many().Invoke(t);
		if (!p.HasValue)
		{
			Assert.Fail(p.ToString());
		}
		Assert.That(p.Value.Length == 3);
	}

	[Test]
	public void SectionListTest()
	{
		var i = """
		        [section_name]
		        translate -10 -10
		        set radius 20
		        r1:rect 10 0 0 20

		        """;
		var t = Tokenize(i);
		var p = ShapeParser.Section.Invoke(t);
		if (!p.HasValue)
		{
			Assert.Fail(p.ToString());
		}
		Assert.That(p.Value.Header.Title == "section_name");
		Assert.That(p.Value.Statements.Length == 3);
	}
}