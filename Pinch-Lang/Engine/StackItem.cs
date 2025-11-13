using CGALDotNetGeometry.Shapes;
using Pinch_Lang.Walker;
using ShapesDeclare.AST;

namespace Pinch_Lang.Engine;

public abstract class StackItem
{
	public Dictionary<string, ValueItem> Properties => _properties;
	protected Dictionary<string, ValueItem> _properties;
	protected Environment _environment;

	protected StackItem(Environment env)
	{
		_environment = env;
	}
	public void SetProperty(string propName, Expression expression)
	{
		LazyInitProperties();
		var val = _environment.ExprWalker.WalkExpression(expression);
		_properties[propName] = val;
	}

	public abstract void SetProperty(string propName, ValueItem item);

	protected void DoSetProperty(string propName, ValueItem item)
	{
		_properties[propName] = item;
	}

	protected void LazyInitProperties()
	{
		if (_properties == null)
		{
			_properties = new Dictionary<string, ValueItem>();
		}
	}

}

public abstract class Shape2D : StackItem
{
	protected Shape2D(Environment env) : base(env)
	{
	}


	protected virtual bool ShapeHasProperty()
	{
		return false;
	}


}
public class Shape2D<T> : Shape2D where T : IShape2f
{
	protected T _shape;
	public Shape2D(Environment env, T shape) : base(env)
	{
		_shape = shape;
	}

	public override void SetProperty(string propName, ValueItem item)
	{
		LazyInitProperties();
		
		//check if OUR shape has this property, then set it as needed.
		DoSetProperty(propName, item);
	}
}

public class Circle : Shape2D<Circle2f>
{
	public Circle(Environment env, Circle2f shape) : base(env, shape)
	{
	}

	public override void SetProperty(string propName, ValueItem item)
	{
		if (propName == "radius")
		{
			var r = ValueItem.AsNumber(item);
			_shape.Radius = (float)r;
		}
		DoSetProperty(propName, item);
	}
}