using System.Diagnostics;
using NetTopologySuite.Geometries;
using Pinch_Lang.Engine;
using ShapesDeclare.AST;
using Environment = Pinch_Lang.Engine.Environment;
namespace Pinch_Lang.Walker;

public class StatementWalker
{
	private readonly Environment _environment;

	public StatementWalker(Environment environment)
	{
		_environment = environment;
	}

	public void Walk(Root root)
	{
		//why not be allowed to declare a [shapes] then a [metadata] then a [shapes] again? just treat them as in one order....
		foreach (var sections in root.Sections)
		{
			_environment.SetSection(sections.Header.Title);
			switch (_environment.SectionType)
			{
				case SectionType.Regular:
				{
					foreach (var statement in sections.Statements)
					{
						WalkStatement(statement);
					}

					break;
				}
				case SectionType.CanvasProperties:
				{
					foreach (var statement in sections.Statements)
					{
						WalkPropertiesStatement(statement);
					}

					break;
				}
				case SectionType.Ignore:
					continue;
				default:
					throw new Exception($"Unknown or Invalid Section Type: {_environment.SectionType}");
			}
		}
	}

	public void WalkPropertiesStatement(Statement statement)
	{
		if (statement is VariableDeclaration vd)
		{
			Console.WriteLine("Walking Property Statement");
			var val = _environment.ExprWalker.WalkExpression(vd.Expression);
			var n = vd.Name;
			_environment.SetMetaProperty(n, val);
		}
		else
		{
			throw new Exception("Invalid Property Statement. Only Variable declarations (prop = val) are allowed.");
		}
	}

	public void WalkStatement(Statement statement)
	{
		switch (statement)
		{
			case ModuleDeclaration modDeclare:
				var m = WalkModuleDeclaration(modDeclare);
				break;
			case ModuleCall functionCall:
				WalkModuleCall(functionCall);
				break;
			case Push push:
				//context
				//_environment.Push(item);
				throw new NotImplementedException();
				break;
			case Pop pop:
				//_environment.Pop();
				throw new NotImplementedException();
				break;
			case GlobalsDeclaration globalsDeclaration:
				if (_environment.IsAtRootFrame)
				{
					throw new Exception("Globals can only be set inside of a {}'s (a stack block)");
				}
				_environment.CurrentFrame.SetGlobals(globalsDeclaration);
				break;
			case StackBlock stackBlock:
				_environment.PushNewFrame();
				foreach (var sub in stackBlock.Statements)
				{
					WalkStatement(sub);
				}
				var f = _environment.PopFrame();
				f.ExitFrame();
				break;
			case VariableDeclaration varDec:
				var varName = varDec.Name;
				var value = _environment.ExprWalker.WalkExpression(varDec.Expression);
				
				if (varName.Prefix == IdentPrefix.Underscore)
				{
					return;
				}else if (varName.Prefix != IdentPrefix.None)
				{
					throw new Exception($"Invalid variable prefix on {varDec}. ({varName.Prefix}) Only '_' (discarded) is allowed.");
				}
				_environment.CurrentFrame.SetLocal(varName.ToString(), value);
				break;
			case MetaStatement ms:
				throw new Exception("Unexpected Meta Statement");
		}
	}
	
	private Module WalkModuleDeclaration(ModuleDeclaration modDec)
	{
		var modName = modDec.Name.ToString();
		var args = modDec.Params;
		return _environment.RegisterModule(modName, args, modDec.Statement);
	}

	private static readonly ValueItem[] EmptyValueItemList = [];

	public ValueItem[] WalkExpressionListToItemList(Expression[] expressions)
	{
		if (expressions.Length == 0)
		{
			return EmptyValueItemList;
		}
		//Walk Arguments and collect them into list.
		ValueItem[] arguments = new ValueItem[expressions.Length];

		for (int i = 0; i < expressions.Length; i++)
		{
			arguments[i] = _environment.ExprWalker.WalkExpression(expressions[i]);
		}

		return arguments;
	}
	private void WalkModuleCall(ModuleCall moduleCall)
	{
		//first, figure out if it's a builtin or a module. The below is for builtins but will work with both....
		if (_environment.TryGetModule(moduleCall.Name.ToString(), out Module module))
		{
			module.Walk(moduleCall.Arguments);
			return;
		}

		var arguments = WalkExpressionListToItemList(moduleCall.Arguments);
		//next we need to figure out how many items to provide. Some items work on the stack without us intervening (like tx, which peeks)
		//but most should be provided items that have been taken off of the stack for them, either from 
		//1. Dot Op (shorthand for argument of 1 more item please, first)
		//2. Argument to directly modify stack
			//this should be in constructor, and modify the 'popFromStack' variable i thiiiiink? wait no then we can't use a variable hmmm
		//3. ShapeStack Provided.
		
		//in that order.
		
		//then walk context and put them on shapeStack
		List<StackItem> context = new List<StackItem>();

		if (moduleCall.PopAllFromStack)
		{
			var item = _environment.CurrentFrame.GetStack();
			context.AddRange(item);
			_environment.CurrentFrame.ClearStack();
			Debug.Assert(moduleCall.PopFromStack == 0, "function call can't be both pop and bang (pop one/pop all)");
		}
		else
		{
			for (int i = 0; i < moduleCall.PopFromStack; i++)
			{
				var item = _environment.CurrentFrame.PopStackItem();
				context.Add(item);
			}
		}
		
		if (moduleCall.StackBlock != null)
		{
			//enter frame and evaluate block, then copy list to our shape list with exit frame.
			//this is the same code as "walk stack block" except that we 'catch' the popped block in order to get it's items.
			_environment.PushNewFrame();
			foreach (var sub in moduleCall.StackBlock.Statements)
			{
				WalkStatement(sub);
			}
			var f = _environment.PopFrame();
			
			foreach (var item in f.GetStack())
			{
				context.Add(item);
			}
			
			f.ExitFrame();
		}
		
		var name = moduleCall.Name.ToString();
		if(Builtins.BuiltinLookup.ContainsKey(name)){
			try
			{
				Builtins.BuiltinLookup[name].Invoke(_environment, arguments, context);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}
		//todo: aliases.
		//else if, walk up the frames for a module with this name.
		else
		{
			throw new Exception($"Unknown Function '{name}'");
		}
	}
}