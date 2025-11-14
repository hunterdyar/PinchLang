using System.Text;
using ShapesDeclare.Utility;
using Superpower.Model;

namespace ShapesDeclare.AST;

public class Section
{
    public Header Header;
    public Statement[] Statements;

    public Section(Header header, Statement[] statements)
    {
        Header = header;
        Statements = statements;
    }

    public override string ToString()
    {
        StringBuilder _sb = new StringBuilder();
        _sb.AppendLine(Header.ToString());
        foreach (var statement in Statements)
        {
            _sb.AppendLine(statement.ToString());
        }

        return _sb.ToString();
    }
}

public class StackBlock : Statement
{
    public Statement[] Statements;
    public StackBlock(params Statement[] statements)
    {
        Statements = statements;
    }
}

public class Header
{
    private TextSpan Value;

    public Header(TextSpan value)
    {
        Value = value;
    }

    public string Title => Value.ToString().ToLower();

    public override string ToString()
    {
        return "[" + Value.ToString() + "]";
    }
}


public class Root
{
    public static Root Empty = new Root([]);
    public Section[] Sections;

    public Root(Section[] sections)
    {
        Sections = sections;
    }

    public override string ToString()
    {
        return Sections.ToStringDelimited("\n");
    }
}