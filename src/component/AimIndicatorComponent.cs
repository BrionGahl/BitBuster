using Godot;

namespace BitBuster.component;

public partial class AimIndicatorComponent : Line2D
{
	[Export]
	public Node2D Parent { get; set; }
	
	[Export]
	public float IndicatorDistance { get; set; }
	
	public void DrawLine(Vector2 location)
	{
		AddPoint(Parent.GlobalPosition);
		
		AddPoint(location.MoveToward(Parent.GlobalPosition, location.DistanceTo(Parent.GlobalPosition) - IndicatorDistance));
	}
}
