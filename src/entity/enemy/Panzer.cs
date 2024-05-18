using System;
using BitBuster.utils;
using Godot;

namespace BitBuster.entity.enemy;

public partial class Panzer : MovingEnemy
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
		AgentTimer.Timeout += OnAgentTimeout;
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

	public override void AttackAction(double delta)
	{
		SetGunRotationAndPosition(Mathf.Pi/12);
		if (CanSeePlayer() && RandomNumberGenerator.Randf() > 0.3f)
			WeaponComponent.AttemptShoot(Player.Position.AngleToPoint(Position) + RandomNumberGenerator.RandfRange(-Mathf.Pi / 9, Mathf.Pi / 9));
	}

	public override void MoveAction(double delta)
	{
		Vector2 goalVector = (Agent.GetNextPathPosition() - GlobalPosition).Normalized();
		if (!IsIdle)
			Rotation = Mathf.LerpAngle(Rotation, (goalVector.Angle() + Constants.HalfPiOffset), RotationSpeed / 60 );
		Velocity = new Vector2((float)(-Speed * Math.Sin(-Rotation)), (float)(-Speed * Math.Cos(-Rotation)));
		
		MoveAndSlide();
	}

	public override void OnAgentTimeout()
	{
		Agent.TargetPosition = Target == Vector2.Zero ? Player.Position : Target;
	}

	private void OnMapReady(Rid rid)
	{
		SetPhysicsProcess(true);
	}

	
}
