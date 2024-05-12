using Godot;

namespace BitBuster.entity.enemy;

public partial class Detonator : Enemy
{
	
	private AnimatedSprite2D _hull;

	public override void _Ready()
	{
		SetPhysicsProcess(false);

		base._Ready();
		_hull = GetNode<AnimatedSprite2D>("Hull");

		NavigationServer2D.MapChanged += OnMapReady;
	}

	public override void SetGunRotationAndPosition(float radian = 0)
	{
	}

	public override void HandleAnimations()
	{
		_hull.Animation = IsIdle ? "default" : "moving";
		_hull.Play();
	}
	
	private void OnMapReady(Rid rid)
	{
		SetPhysicsProcess(true);
	}
}
