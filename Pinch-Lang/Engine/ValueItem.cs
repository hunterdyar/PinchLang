using System.Linq.Expressions;
using Superpower.Model;

namespace Pinch_Lang.Engine;

public abstract class ValueItem
{
	public abstract object NativeValue { get; } 
	public double AsNumber()
	{
		switch (this)
		{
			case NumberValue numberValue:
				return numberValue.Value;
			default:
				throw new InvalidCastException($"Cannot convert {this} to number");
		}
	}

	public string AsStringOrID()
	{
		switch (this)
		{
			case StringValue sv:
				return sv.Value;
			case IdentifierValue iv:
				return iv.Value;
			default:
				throw new InvalidCastException($"Cannot convert {this} to name or string");
		}
	}
	public abstract bool IsNumeric(Pinch_Lang.Engine.Environment env);
}

public class NumberValue : ValueItem
{
	public double Value
	{
		get => _value;
		set => _value = value;
	}

	public override object NativeValue
	{
		get { return _value; }
	}

	public override bool IsNumeric(Environment env)
	{
		return true;
	}

	private double _value;

	public NumberValue(double val)
	{
		_value = val;
	}
}

public class StringValue : ValueItem
{
	public string Value
	{
		get => _value;
		set => _value = value;
	}

	public override object NativeValue
	{
		get { return _value; }
	}

	public override bool IsNumeric(Environment env)
	{
		return false;
	}

	private string _value;

	public StringValue(TextSpan value)
	{
		_value = value.ToStringValue();
	}
	public StringValue(string value)
	{
		_value = value;
	}
}

public class ListValue : ValueItem
{
	public ValueItem[] Items;
	public ListValue(ValueItem[] values)
	{
		Items = values;
	}

	public override object NativeValue => Items.Select(x=>x.NativeValue).ToArray();
	public override bool IsNumeric(Environment env)
	{
		return false;
	}

	public bool IsPoint(Environment env)
	{
		return Items.Length == 2 && Items[0].IsNumeric(env) && Items[1].IsNumeric(env);
	}
}

public class IdentifierValue : ValueItem
{
	public string Value
	{
		get => _value;
		set => _value = value;
	}
	
	public override object NativeValue
	{
		get { return _value; }
	}

	public override bool IsNumeric(Environment env)
	{
		if (env.CurrentFrame.TryGetValueItem(Value, out var item))
		{
			return item.IsNumeric(env);
		}

		return false;
	}

	private string _value;

	public IdentifierValue(TextSpan value)
	{
		_value = value.ToStringValue();
	}
}