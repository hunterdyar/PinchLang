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
			foreach (var statement in sections.Statements)
			{
				WalkStatement(statement);
			}
		}
	}

	private void WalkStatement(Statement statement)
	{
		switch (statement)
		{
			case ModuleDeclaration modDeclare:
				var m = WalkModuleDeclaration(modDeclare);
				break;
			case FunctionCall functionCall:
				WalkFunctionCall(functionCall);
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
		}
	}
	
	private Module WalkModuleDeclaration(ModuleDeclaration modDec)
	{
		var modName = modDec.Name.ToString();
		var args = modDec.Params;
		return _environment.RegisterModule(modName, args, modDec.Statement);
	}

	private void WalkFunctionCall(FunctionCall functionCall)
	{
		switch (functionCall.Name.ToString())
		{
			case "set":
				Builtins.Set(_environment, functionCall.Arguments);
				break;
			default:
				throw new Exception($"Unknown function or module '{functionCall.Name}'");
		}
	}
}