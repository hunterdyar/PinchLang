using ShapesDeclare;
using ShapesDeclare.AST;
using ShapesDeclare.Utility;
using Superpower;
using Superpower.Model;

namespace PinchLangTests;

public class ParserTests
{
	public static TokenList<SToken> Tokenize(string input)
	{
		var tokr = ShapeTokenizer.Tokenizer.TryTokenize(input);

		if (!tokr.HasValue)
		{
			Assert.Fail(tokr.ToString());
		}

		return tokr.Value;
	}

	[TestCase("translate (-10) 3 + 4")]
	[TestCase("translate (-10) - 10")]
	[TestCase("translate (-10) + 10 + 10")]
	[TestCase("translate 5")]
	[TestCase("translate 1 + 2 * 4")]

	public void ModuleCallWithExpressionsTest(string i)
	{
		var t = Tokenize(i);
		var p = ShapeParser.Statement.Invoke(t);
		if (!p.HasValue)
		{
			Assert.Fail("fail at modCallFirstPart"+p.ToString());
		}
		Console.WriteLine($"1. statement parsed as {p.Value.GetType()}");
		Assert.That(p.Value.ToString().Trim(), Is.EqualTo(i.Trim()));
		Console.WriteLine($"check");

		var p3 = ShapeParser.ModuleCallNoBlock.Invoke(t);
		if (!p3.HasValue)
		{
			Assert.Fail("fail at modCallNoBlock"+p3.ToString());
		}

		Console.WriteLine($"3. statement parsed as {p.Value.GetType()}");
		Assert.That(p3.Value.ToString().Trim(), Is.EqualTo(i.Trim()));
		Console.WriteLine($"check");

	}
	
	[Test]
	public void ModuleCallTest()
	{
		var i = """
		        set radius 20
		        round r1 20
		        translate 10 10
		        """;
		var t = Tokenize(i);
		var p = ShapeParser.ModuleCallNoBlock.Many().Invoke(t);
		if (!p.HasValue)
		{
			Assert.Fail(p.ToString());
		}

		var output = p.Value.ToStringDelimited(Environment.NewLine).Trim();
		Assert.That(output, Is.EqualTo(i.Trim()));
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
		 
	
		        
		        {rect a b}
		        
		        {
		          another:hello 20 {}
		        }
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
	public void ModuleWithBlockDecTest()
	{
		var i = """
		        .difference 0 0{
		        set a b
		        }

		        """;
		var t = Tokenize(i);
		var p = ShapeParser.ModuleCallWithBlock.Many().Invoke(t);
		if (!p.HasValue)
		{
			Assert.Fail(p.ToString());
		}

		var fn = (ModuleCall)p.Value[0];
		
		Assert.That(fn.Name.ToString(), Is.EqualTo("difference"));
		Assert.That(fn.Identifier.Prefix, Is.EqualTo(IdentPrefix.Dot));
		Assert.That(fn.PopFromStack, Is.EqualTo(1));
		Assert.That(fn.Arguments.Length, Is.EqualTo(2));
		Assert.That(fn.StackBlock?.Statements.Length, Is.EqualTo(1));

		//now with no whitespace
		
		 i = """.difference 0 0{set a b}""";
		t = Tokenize(i);
		p = ShapeParser.ModuleCallWithBlock.Many().Invoke(t);
		if (!p.HasValue)
		{
			Assert.Fail(p.ToString());
		}

		fn = (ModuleCall)p.Value[0];

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

	[TestCase("test:rect", "test", new string[0])]
	[TestCase("test:@a @b rect", "test", new string[]{"a","b"})]
	[TestCase("""
	          test:@a @b {
	          rect a a b b
	          }
""", "test", new string[] { "a", "b" })]
	public void ModuleDeclarationTest(string input, string expectedName, string[] expectedParams)
	{
		var t = Tokenize(input);
		var p = ShapeParser.ModuleDeclaration.Invoke(t);
		if (!p.HasValue)
		{
			Assert.Fail(p.ToString());
		}

		var md = (ModuleDeclaration)p.Value;
		Assert.That(md.Name.ToString(), Is.EqualTo(expectedName));
		Assert.That(md.Params.Select(x=>x.Value.ToString()).ToArray(), Is.EqualTo(expectedParams));
	}

	[TestCase("a = 2","a = 2")]
	public void VariableDeclarationTest(string input, string expectedOutput)
	{
		var t = Tokenize(input);
		var p = ShapeParser.VariableDeclaration.Invoke(t);
		if (!p.HasValue)
		{
			Assert.Fail(p.ToString());
		}

		var md = (VariableDeclaration)p.Value;
		Assert.That(md.ToString(), Is.EqualTo(expectedOutput));
	}

	[Test]
	public void MultipleModuleDecsTest()
	{
		var i = """
		        stamp:circle 5 5 5
		        dance: @a circle 55
		        jump: @a @b {}

		        """;
		var t = Tokenize(i);
		var p = ShapeParser.Statement.Many().Invoke(t);
		if (!p.HasValue)
		{
			Assert.Fail(p.ToString());
		}

		var md = p.Value.Cast<ModuleDeclaration>().ToArray();
		Assert.That(md.Length, Is.EqualTo(3));
	}
	[Test]
	public void SectionListTest1()
	{
		var i = """
		        [section_name]
		        translate 10 10
		        set radius 20
		        banana = 3
		        r1:rect banana 
		        
		        """;
		var t = Tokenize(i);
		var p = ShapeParser.Section.Invoke(t);
		if (!p.HasValue)
		{
			Assert.Fail(p.ToString());
		}
		Assert.That(p.Value.Header.Title == "section_name");
		Assert.That(p.Value.Statements.Length == 4);
	}

	[Test]
	public void SectionListTest2()
	{
		var i = """
		        [canvas]
		        x = 20
		        y = 30
		        rect 0 y+30 x y

		        """;
		var t = Tokenize(i);
		var p = ShapeParser.Section.Invoke(t);
		if (!p.HasValue)
		{
			Assert.Fail(p.ToString());
		}

		Assert.That(p.Value.Header.Title == "canvas");
		Assert.That(p.Value.Statements.Length == 3);
	}
	
	
	[Test]
	public void StatementMisc()
	{
		var i = """
		        r1:rect
		        """;
		var t = Tokenize(i);
		var p = ShapeParser.Statement.Many().Invoke(t);
		if (!p.HasValue)
		{
			Assert.Fail(p.ToString());
		}
		
	}
}