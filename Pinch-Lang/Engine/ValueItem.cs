using Superpower.Model;

namespace Pinch_Lang.Engine;

public class ValueItem
{
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
}

public class NumberValue : ValueItem
{
	public double Value
	{
		get => _value;
		set => _value = value;
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

	private string _value;

	public StringValue(TextSpan value)
	{
		_value = value.ToStringValue();
	}
}

public class IdentifierValue : ValueItem
{
	public string Value
	{
		get => _value;
		set => _value = value;
	}

	private string _value;

	public IdentifierValue(TextSpan value)
	{
		_value = value.ToStringValue();
	}
}