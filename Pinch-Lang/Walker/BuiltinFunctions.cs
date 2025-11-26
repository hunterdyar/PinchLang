using System.Text;
using Pinch_Lang.Engine;
using Environment = Pinch_Lang.Engine.Environment;

namespace Pinch_Lang.Walker;

public class BuiltinFunctions
{
	public static readonly Dictionary<string, Func<Environment, ValueItem[], ValueItem>> Builtins =
	new Dictionary<string, Func<Environment, ValueItem[], ValueItem>>(){
		//single argument math functions
		{"sin", CreateSingleArgNumberFunc("sin", Math.Sin) },
		{ "asin", CreateSingleArgNumberFunc("asin", Math.Asin) },
		{ "cos", CreateSingleArgNumberFunc("cos", Math.Cos) },
		{ "acos", CreateSingleArgNumberFunc("acos", Math.Acos) },
		{ "tan", CreateSingleArgNumberFunc("tan", Math.Tan) },
		{ "atan", CreateSingleArgNumberFunc("atan", Math.Atan) },

		{ "abs", CreateSingleArgNumberFunc("abs", Math.Abs) },
		{ "cbrt", CreateSingleArgNumberFunc("cuberoot", Math.Cbrt) },
		{ "cuberoot", CreateSingleArgNumberFunc("cuberoot", Math.Cbrt) },
		{ "ceiling", CreateSingleArgNumberFunc("ceiling", Math.Ceiling) },
		{ "exp", CreateSingleArgNumberFunc("exp", Math.Exp) },
		{ "floor", CreateSingleArgNumberFunc("floor", Math.Floor) },
		{ "sqrt", CreateSingleArgNumberFunc("sqrt", Math.Sqrt) },
		{ "squareroot", CreateSingleArgNumberFunc("sqrt", Math.Sqrt) },
		{ "sign", CreateSingleArgNumberFunc("sign", (f)=>(double)Math.Sign(f)) },
		{ "round", CreateSingleArgNumberFunc("round", Math.Round) },
		{ "log", Log },
		{ "log2", CreateSingleArgNumberFunc("log2", Math.Log2) },
		{ "log10", CreateSingleArgNumberFunc("log10", Math.Log10) },
		
		//double funcs
		{ "max", CreateDoubleArgNumberFunc("max", Math.Max) },
		{ "min", CreateDoubleArgNumberFunc("max", Math.Min) },
		{ "pow", CreateDoubleArgNumberFunc("max", Math.Pow, "a", "x") },
		{ "copy_sign", CreateDoubleArgNumberFunc("copy_sign", Math.CopySign, "magOf", "signOf") },

	};

	public static bool ValidateArguments(string funcName, ValueItem[] provided, string[][] signatures)
	{
		var valid = signatures.Any(x => x.Length == provided.Length);
		if (!valid)
		{
			StringBuilder err = new StringBuilder();
			err.Append($"Bad number of arguments for function {funcName}. Expected ");
			if (signatures.Length == 0)
			{
				err.Append('0');
			}
			else
			{
				for (var i = 0; i < signatures.Length; i++)
				{
					var sig = signatures[i];
					err.Append(sig.Length);
					err.Append(' ');
					if (sig.Length > 0)
					{
						err.Append('(');
						for (int j = 0; j < sig.Length; j++)
						{
							err.Append(sig[j]);
							if (j < sig.Length - 1)
							{
								err.Append(' ');
							}
						}

						err.Append(')');
					}

					if (i < signatures.Length - 1)
					{
						err.Append(" or ");
					}
				}
			}

			err.Append(". Got ");
			err.Append(provided.Length);
			err.Append('.');
			throw new Exception(err.ToString());
		}

		return valid;
	}

	#region Helpers
	public static Func<Environment, ValueItem[], ValueItem> CreateSingleArgNumberFunc(string name, Func<double, double> f, string argName = "input")
	{
		return ((environment, args) =>
		{
			ValidateArguments(name, args, [[argName]]);
			var val = args[0].AsNumber();
			return new NumberValue(f.Invoke(val));
		});
	}

	public static Func<Environment, ValueItem[], ValueItem> CreateDoubleArgNumberFunc(string name, Func<double, double, double> f, string argAName = "a", string argBName = "b")
	{
		return ((environment, args) =>
		{
			ValidateArguments(name, args, [[argAName, argBName]]);
			var a = args[0].AsNumber();
			var b = args[1].AsNumber();
			return new NumberValue(f.Invoke(a,b));
		});
	}
	

	#endregion

	public static ValueItem Log(Environment env, ValueItem[] args)
	{
		ValidateArguments("log", args, [["d"], ["a", "new base"]]);
		var a = args[0].AsNumber();
		if (args.Length == 1)
		{
			return new NumberValue(Math.Log(a));
		}else if (args.Length == 2)
		{
			var b = args[1].AsNumber();
			return new NumberValue(Math.Log(a, b));
		}
		else
		{
			throw new Exception("uh oh");
		}
	}
}