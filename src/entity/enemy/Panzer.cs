using BitBuster.utils;
using Godot;

namespace BitBuster.entity.enemy;

public partial class Panzer : Enemy
{
	private Sprite2D _gun;
	private AnimatedSprite2D _hull;

	public override void _Ready()
	{
		SetPhysicsProcess(false);

		base._Ready();
		_gun = GetNode<Sprite2D>("Gun");
		_hull = GetNode<AnimatedSprite2D>("Hull");

		NavigationServer2D.MapChanged += OnMapReady;
	}

	public override void SetGunRotationAndPosition(float radian = 0)
	{
		if (CanSeePlayer())
			_gun.Rotation = (float)Mathf.LerpAngle(_gun.Rotation, Player.Position.AngleToPoint(Position) - Constants.HalfPiOffset, 0.5);
		else
			_gun.Rotation = (float)Mathf.LerpAngle(_gun.Rotation, _gun.Rotation + radian, 0.1);
		_gun.Position = Position;
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
