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

		float dist = location.DistanceTo(Parent.GlobalPosition);

		
		if (dist > IndicatorDistance)
		{
			
		}
		
		location.MoveToward(Parent.GlobalPosition, dist);
		AddPoint(location.MoveToward(Parent.GlobalPosition, dist - IndicatorDistance));
	}
}
