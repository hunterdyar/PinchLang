namespace Pinch_Lang.Engine;

public struct CanvasProperties
{
	public double Width = 100;
	public double Height = 100;
	public CoordinateTransform CoordinateTransform = CoordinateTransform.None;
	public DegreeUnits DegreeUnits = DegreeUnits.Degrees;
	public string CanvasSection = "canvas";
	public string[] IgnoreSections = new[] { "ignore" };
	public CanvasProperties()
	{
	}

	public bool IsCanvasSection(string sectionName)
	{
		return sectionName == CanvasSection;
	}

	public bool IsIgnoreSection(string sectionName)
	{
		return IgnoreSections.Contains(sectionName);
	}
}

public enum CoordinateTransform
{
	None,
	FlipY,
	FitToBounds,
}

public enum DegreeUnits
{
	Degrees,
	Radians
} 