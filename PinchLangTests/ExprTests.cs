using ShapesDeclare;
using ShapesDeclare.AST;
using  Environment = Pinch_Lang.Engine.Environment; 
namespace PinchLangTests;

public class ExprTests
{
    [TestCase("1+1", 2)]
    [TestCase("1-2", -1)]
    [TestCase("1+2*3", 7)]
    [TestCase("\"he\" + \"llo\"", "hello")]

    public void BinaryOperatorTests(string input, object expected)
    {
        var t = ParserTests.Tokenize(input);

        var p = ShapeParser.Expression.Invoke(t);
        if (!p.HasValue)
        {
            Assert.Fail(p.ToString());
        }

        var e = new Environment();
        var expr = p.Value;
        var item = e.ExprWalker.WalkExpression(expr);
        Assert.That(item.NativeValue, Is.EqualTo(expected));
    }
}