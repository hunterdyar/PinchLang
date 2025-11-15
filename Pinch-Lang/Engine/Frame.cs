namespace Pinch_Lang.Engine;

public class Frame
{
    public Frame? Parent;

    private Stack<StackItem> _stack = new Stack<StackItem>();
    
    private Dictionary<string, ValueItem> _items;
    
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
}