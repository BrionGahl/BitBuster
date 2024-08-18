using Godot;

namespace BitBuster.component;

public partial class LineTrailComponent : Line2D
{
	public Node2D Parent { get; set; }

	[Export]
	public int TrailLength { get; set; }

	public override void _Ready()
	{
		Parent = GetParent<Node2D>();
	}


	public override void _Process(double delta)
	{
		AddPoint(Parent.GlobalPosition);
		if (Points.Length > TrailLength)
			RemovePoint(0);
	}
}
