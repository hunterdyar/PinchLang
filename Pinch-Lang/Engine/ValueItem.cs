using Superpower.Model;

namespace Pinch_Lang.Engine;

public class ValueItem
{
	public static double AsNumber(ValueItem valueItem)
	{
		switch (valueItem)
		{
			case NumberValue numberValue:
				return numberValue.Value;
			default:
				throw new InvalidCastException($"Cannot convert {valueItem} to number");
		}
	}

	public static string AsStringOrID(ValueItem item)
	{
		switch (item)
		{
			case StringValue sv:
				return sv.Value;
			case IdentifierValue iv:
				return iv.Value;
			default:
				throw new InvalidCastException($"Cannot convert {item} to name or string");
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


public class ColorValue : ValueItem
{
	public CGALDotNetGeometry.Colors.ColorRGBA Value
	{
		get => _value;
		set => _value = value;
	}

	private CGALDotNetGeometry.Colors.ColorRGBA _value;
}