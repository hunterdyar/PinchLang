using ShapesDeclare.AST;

namespace Pinch_Lang.Engine;

public class Frame
{
    public Environment Environment => _environment;
    private Environment _environment;
    public readonly Frame? Parent;
    private readonly Stack<StackItem> _stack = new Stack<StackItem>();
    private readonly Dictionary<string, ValueItem> _items = new Dictionary<string, ValueItem>();
    private readonly Dictionary<string, Module> _modules = new Dictionary<string, Module>();
    private readonly List<string> _globals = new List<string>();

    public Frame(Environment environment)
    {
        _environment = environment;
        Parent = _environment.CurrentFrame;
    }

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

    public bool TryGetModule(string identifier, out Module module)
    {
        if (_modules.TryGetValue(identifier, out module))
        {
            return true;
        }

        if (Parent != null)
        {
            return Parent.TryGetModule(identifier, out module);
        }

        module = null;
        return false;
    }
    
    
    //configure locals, give default values?
    public void EnterFrame()
    {
        
    }

    public void ShiftTopToParentStack()
    {
        if (_stack.Count > 0)
        {
            Parent?._stack.Push(_stack.Pop());
        }
    }
    //reset, etc so we can do an object pool.
    public void ExitFrame()
    {
        ShiftTopToParentStack();
        _stack.Clear();
        _items.Clear();
        _globals.Clear();
        _modules.Clear();
    }

    public void PushStackItem(StackItem item)
    {
        _stack.Push(item);
    }

    public StackItem PopStackItem()
    {
        if(!_stack.TryPop(out var p)){
            throw new Exception("Could not pop stack item, no items in local frame.");
        }

        return p;
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

    public void SetLocal(string name, ValueItem value)
    {
        if (!_items.TryAdd(name, value))
        {
            _items[name] = value;
            return;
        }
    }

    public void RegisterModule(Module mod)
    {
        if (_modules.ContainsKey(mod.Name))
        {
            throw new Exception($"Error! Can't declare module {mod.Name}, module with that name already declared");
        }
        _modules.Add(mod.Name, mod);
    }

    public List<StackItem> GetStack()
    {
        //todo: does ToList allocate much here?
        return _stack.ToList();
    }
}