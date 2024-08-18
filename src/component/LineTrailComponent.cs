using Godot;

namespace BitBuster.component;

public partial class LineTrailComponent : Line2D
{
	private Node2D _parent;
	private int _trailLength;

	public Node2D Parent
	{
		get => _parent;
		set => _parent = value;
	}
	
	[Export]
	public int TrailLength { 
		get => _trailLength;
		set => _trailLength = value; }
	
	public override void _Ready()
	{
		_parent = GetParent<Node2D>();
	}


	public override void _Process(double delta)
	{
		AddPoint(_parent.GlobalPosition);
		if (Points.Length > TrailLength)
			RemovePoint(0);
	}
}
