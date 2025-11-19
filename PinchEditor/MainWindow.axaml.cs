using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
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
	
	WindowModel _wm => DataContext as WindowModel;
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
		_textEditor.Options.ShowTabs = true;
		_textEditor.Options.ShowSpaces = true;
		_textEditor.Text = "[canvas]\nrect 0 0 50 50";
		
		_svg.Stretch = Stretch.Fill;
		_svg.EnableCache = false;
		
		// _textEditor.TextArea.AddHandler(PointerWheelChangedEvent, OnPointerWheelChanged());
		
	}

	protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
	{
		if (e.KeyModifiers == KeyModifiers.Control)
		{
			if (e.Delta.Y > 0)
			{
				_textEditor.FontSize += 2;
				e.Handled = true;
				return;
			}

			if (e.Delta.Y < 0)
			{
				if (_textEditor.FontSize > 6)
				{
					_textEditor.FontSize -= 2;
				}

				e.Handled = true;
				return;
			}
		}

		base.OnPointerWheelChanged(e);
	}

	private void TextEditorOnTextChanged(object? sender, EventArgs e)
	{
		Render();
	}

	public void Render()
	{
		
		//clear list of error messages
		var p = ShapeParser.TryParse(_textEditor.Text, out Root root, out var error);
		if (!p)
		{
			//add to list of error messages for a scrollbox.
			return;
		}

		var e = new Pinch_Lang.Engine.Environment();
		var result =  e.Execute(root);
		if (result.DidSucceed)
		{
			//this is failing because we aren't pulling the stack from the shapes 'up to' the root item.
			//which is intended! or, well it's not. I just haven't designed that yet. 
			var source = EnvUtil.SvgDocumentToString(result.Document);
			_svg.Source = source;
		}
		else
		{
			Console.WriteLine(result.Error.Message);
		}

	}
}