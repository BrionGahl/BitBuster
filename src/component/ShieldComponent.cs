using Godot;

namespace BitBuster.component;

public partial class ShieldComponent : CharacterBody2D
{
	private float _speed = 100f;

	public override void _PhysicsProcess(double delta)
	{
		RotationDegrees += _speed * (float)delta % 360f;
	}
}
