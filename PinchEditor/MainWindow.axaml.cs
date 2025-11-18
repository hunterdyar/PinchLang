using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using AvaloniaEdit;
using PinchEditor.ViewModels;
using ShapesDeclare;
using ShapesDeclare.AST;
using ShapesDeclare.Utility;

namespace PinchEditor;

public partial class MainWindow : Window
{
	private readonly TextEditor _textEditor;
	private readonly Avalonia.Svg.Skia.Svg _svg;
	public MainWindow()
	{
		InitializeComponent();
		_textEditor = this.FindControl<TextEditor>("Editor");
		_svg = this.FindControl<Avalonia.Svg.Skia.Svg>("Preview");
		_textEditor.TextChanged += TextEditorOnTextChanged;
		_textEditor.ShowLineNumbers = true;
		_textEditor.Options.ShowColumnRulers = true;
		_textEditor.Options.ShowBoxForControlCharacters = true;
		_textEditor.Options.CutCopyWholeLine = true;
		_svg.Stretch = Stretch.Fill;
		
	}

	private void TextEditorOnTextChanged(object? sender, EventArgs e)
	{
		Render();
	}

	public void Render()
	{
		try
		{
			//clear list of error messages
			var p = ShapeParser.TryParse(_textEditor.Text, out Root root, out var error);
			if (!p)
			{
				//add to list of error messages for a scrollbox.
				return;
			}

			var e = new Pinch_Lang.Engine.Environment();
			var svg = e.Execute(root);
			//this is failing because we aren't pulling the stack from the shapes 'up to' the root item.
			//which is intended! or, well it's not. I just haven't designed that yet. 
			var source= EnvUtil.SvgDocumentToString(svg);
			_svg.Source = source;
		}
		catch (Exception exception)
		{
			Console.WriteLine(exception);
			throw;
		}
		
	}
}