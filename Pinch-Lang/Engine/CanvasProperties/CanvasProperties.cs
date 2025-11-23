namespace Pinch_Lang.Engine;

public struct CanvasProperties
{
	public double Width = 100;
	public double Height = 100;
	public CoordinateTransform CoordinateTransform = CoordinateTransform.None;
	public DegreeUnits DegreeUnits = DegreeUnits.Degrees;
	public CanvasProperties()
	{
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