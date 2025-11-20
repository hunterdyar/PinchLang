using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls.PanAndZoom;
using AvaloniaEdit;
using AvaloniaEdit.Editing;
using Pinch_Lang.Engine;
using ShapesDeclare;
using ShapesDeclare.AST;
using ShapesDeclare.Utility;

namespace PinchEditor.ViewModels;

public class WindowModel
{
	public string SvgSource { get; set; } = "";
	public ObservableCollection<ResultMessage> Console { get; set; } = new ObservableCollection<ResultMessage>();
	public void CopyMouseCommand(TextArea textArea)
	{
		ApplicationCommands.Copy.Execute(null, textArea);
	}

	public void CutMouseCommand(TextArea textArea)
	{
		ApplicationCommands.Cut.Execute(null, textArea);
	}

	public void PasteMouseCommand(TextArea textArea)
	{
		ApplicationCommands.Paste.Execute(null, textArea);
	}

	public void SelectAllMouseCommand(TextArea textArea)
	{
		ApplicationCommands.SelectAll.Execute(null, textArea);
	}

	// Undo Status is not given back to disable it's item in ContextFlyout; therefore it's not being used yet.
	public void UndoMouseCommand(TextArea textArea)
	{
		ApplicationCommands.Undo.Execute(null, textArea);
	}

	public void FontSizeIncreaseCommand(TextEditor area)
	{
		area.FontSize += 2;
	}

	public void FontSizeDecreaseCommand(TextEditor area)
	{
		if (area.FontSize > 6)
		{
			area.FontSize -= 2;
		}
	}

	public void Recenter(ZoomBorder zoomBorder)
	{
		zoomBorder.AutoFit();
	}
}