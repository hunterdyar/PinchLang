using ShapesDeclare;
using ShapesDeclare.AST;
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
		var p = ShapeParser.FunctionCallNoBlock.Many().Invoke(t);
		if (!p.HasValue)
		{
			Assert.Fail(p.ToString());
		}
		Assert.That(p.Value.Length == 3);
	}

	[Test]
	public void StackBlockSingleTest()
	{
		var i = """
		        {

		        }
		        """;
		var t = Tokenize(i);
		var p = ShapeParser.StackBlock.Invoke(t);
		if (!p.HasValue)
		{
			Assert.Fail(p.ToString());
		}
	}

	[Test]
	public void StackBlockTest()
	{
		var i = """
		        {

		        }
		        {
		        r1:rect a b
		        }{rect a b}
		        {
		        diff a b {circle 20}
		        .difference a b c
		        circle c
		        }
		        """;
		var t = Tokenize(i);
		var p = ShapeParser.StackBlock.Many().Invoke(t);
		if (!p.HasValue)
		{
			Assert.Fail(p.ToString());
		}

		var empty = (StackBlock)p.Value[0];
		var one = (StackBlock)p.Value[1];
		var two = (StackBlock)p.Value[2];
		var three = (StackBlock)p.Value[3];

		Assert.That(empty.Statements.Length, Is.EqualTo(0));
		Assert.That(one.Statements.Length, Is.EqualTo(1));
		Assert.That(two.Statements.Length, Is.EqualTo(1));
		Assert.That(three.Statements.Length, Is.EqualTo(3));

	}
	
	[Test]
	public void FunctionWithBlockDecTest()
	{
		var i = """
		        .difference 0 0{
		        set a b
		        }

		        """;
		var t = Tokenize(i);
		var p = ShapeParser.FunctionCallWithBlock.Many().Invoke(t);
		if (!p.HasValue)
		{
			Assert.Fail(p.ToString());
		}

		var fn = (FunctionCall)p.Value[0];
		
		Assert.That(fn.Name.ToString(), Is.EqualTo("difference"));
		Assert.That(fn.Identifier.Prefix, Is.EqualTo(IdentPrefix.Dot));
		Assert.That(fn.PopFromStack, Is.EqualTo(1));
		Assert.That(fn.Arguments.Length, Is.EqualTo(2));
		Assert.That(fn.StackBlock?.Statements.Length, Is.EqualTo(1));

		//now with no whitespace
		
		 i = """.difference 0 0{set a b}""";
		t = Tokenize(i);
		p = ShapeParser.FunctionCallWithBlock.Many().Invoke(t);
		if (!p.HasValue)
		{
			Assert.Fail(p.ToString());
		}

		fn = (FunctionCall)p.Value[0];

		Assert.That(fn.Name.ToString(), Is.EqualTo("difference"));
		Assert.That(fn.Identifier.Prefix, Is.EqualTo(IdentPrefix.Dot));
		Assert.That(fn.PopFromStack, Is.EqualTo(1));
		Assert.That(fn.Arguments.Length, Is.EqualTo(2));
		Assert.That(fn.StackBlock?.Statements.Length, Is.EqualTo(1));
	}

	[Test]
	public void Identifiers()
	{
		var i = """
		        basic BASIC _ _word .dot
		        """;
		
		var t = Tokenize(i);
		var p = ShapeParser.Identifier.Many().Invoke(t);
		if (!p.HasValue)
		{
			Assert.Fail(p.ToString());
		}
		
		Assert.That(p.Value.Length == 5);
		Assert.That(p.Value[0].ToString(), Is.EqualTo("basic"));
		Assert.That(p.Value[1].ToString(), Is.EqualTo("BASIC"));
		Assert.That(p.Value[2].ToString(), Is.EqualTo("_"));
		Assert.That(p.Value[3].ToString(), Is.EqualTo("word"));
		Assert.That(p.Value[4].ToString(), Is.EqualTo("dot"));
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