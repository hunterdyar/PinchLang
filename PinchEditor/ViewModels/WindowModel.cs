using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Platform.Storage;
using AvaloniaEdit;
using AvaloniaEdit.CodeCompletion;
using AvaloniaEdit.Editing;
using Pinch_Lang.Engine;


namespace PinchEditor.ViewModels;

public class WindowModel
{
	public IStorageFile? CurrentFile { get; set; }
	
	public string SvgSource { get; set; } = "";
	public ObservableCollection<ResultMessage> Console { get; set; } = new ObservableCollection<ResultMessage>();
	CompletionWindow completionWindow;
	public void RequestCodeComplete(TextArea textArea)
	{
		completionWindow = new CompletionWindow(textArea);
		IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
		data.Add(new SimpleCompletionData("Item1"));
		data.Add(new SimpleCompletionData("Item2"));
		data.Add(new SimpleCompletionData("Item3"));
		completionWindow.Show();
		completionWindow.Closed += delegate { completionWindow = null; };
	}
  
	
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

	public void OpenFile(IStorageFile file)
	{
		System.Console.WriteLine("open "+file.Path);
	}
}