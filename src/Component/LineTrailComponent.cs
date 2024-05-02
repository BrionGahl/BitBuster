using System.Collections.Generic;
using BitBuster.utils;
using Godot;

namespace BitBuster.component;

public partial class LineTrailComponent : Line2D
{
	private Node2D _parent;
	
	[Export]
	public int TrailLength { get; set; }
	
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
