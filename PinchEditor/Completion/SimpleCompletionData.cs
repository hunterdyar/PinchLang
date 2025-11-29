using System;
using Avalonia.Media;
using AvaloniaEdit.CodeCompletion;
using AvaloniaEdit.Document;
using AvaloniaEdit.Editing;

namespace PinchEditor;
//http://avalonedit.net/documentation/html/47c58b63-f30c-4290-a2f2-881d21227446.htm
public class SimpleCompletionData : ICompletionData
{
	public SimpleCompletionData(string name)
	{
		Text = name;
		Description = "Description for "+name;
		Priority = 10;
	}

	public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
	{
		textArea.Document.Replace(completionSegment, this.Text);
	}

	public IImage Image
	{
		get { return null; }
	}
	public string Text { get; }

	public object Content
	{
		// Use this property if you want to show a fancy UIElement in the list.
		get { return this.Text; }
	}
	public object Description { get; }
	public double Priority { get; }
}