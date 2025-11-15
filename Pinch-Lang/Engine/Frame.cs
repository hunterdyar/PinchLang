using ShapesDeclare.AST;

namespace Pinch_Lang.Engine;

public class Frame
{
    public Frame? Parent;
    private readonly Stack<StackItem> _stack = new Stack<StackItem>();
    private readonly Dictionary<string, ValueItem> _items;

    private readonly List<string> _globals = new List<string>();
    
    public bool TryGetValueItem(string identifier, out ValueItem item)
    {
        if (_items.TryGetValue(identifier, out item))
        {
            return true;
        }

        if (Parent != null)
        {
            return Parent.TryGetValueItem(identifier, out item);
        }

        item = null;
        return false;
    }

    
    
    //configure locals, give default values?
    public void EnterFrame()
    {
        
    }

    //reset, etc so we can do an object pool.
    public void ExitFrame()
    {
        _stack.Clear();
        _items.Clear();
        _globals.Clear();
    }

    public void PushStackItem(StackItem item)
    {
        _stack.Push(item);
    }

    public void PopStackItem()
    {
        if(!_stack.TryPop(out var p)){
            throw new Exception("Could not pop stack item, no items in local frame.");
        }
    }

    public StackItem TopStackItem()
    {
        return _stack.Peek();
    }

    public void SetGlobals(GlobalsDeclaration globalsDeclaration)
    {
        foreach (var id in globalsDeclaration.Identifiers)
        {
            //todo: check that the global exists in the environment root frame. throw a warning or error if it doesn't.
            _globals.Add(id.Value.ToString());
        }
    }
}