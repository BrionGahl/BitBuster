using System;
using BitBuster.component;
using BitBuster.entity.player;
using BitBuster.utils;
using Godot;

namespace BitBuster.entity.enemy;

public partial class Panzer : Enemy
{
	private float Speed
	{
		get => _statsComponent.Speed;
		set => _statsComponent.Speed = value;
	}
	private float RotationSpeed => Speed / 25;
	private bool IsIdle => Velocity.Equals(Vector2.Zero);

	private Sprite2D _gun;
	private AnimatedSprite2D _hull;
	private NavigationAgent2D _agent;
	private Timer _agentTimer;

	private Vector2 _movementDirection;
	private float _rotationDirection;
	
	public override void _Ready()
	{
		_gun = GetNode<Sprite2D>("Gun");
		_hull = GetNode<AnimatedSprite2D>("Hull");
		_agent = GetNode<NavigationAgent2D>("Agent");
		_agentTimer = GetNode<Timer>("Agent/Timer");
		
		SetPhysicsProcess(false);

		_agentTimer.Timeout += FindPath;
		NavigationServer2D.MapChanged += OnNavServerReady;
	}

	public override void _Process(double delta)
	{
		if (!_agent.IsTargetReachable())
			return;
		
		SetGunRotationAndPosition();
		HandleAnimations();
		
		Vector2 goalVector = (_agent.GetNextPathPosition() - GlobalPosition).Normalized();
		if (!IsIdle)
			Rotation = (float)Mathf.LerpAngle(Rotation, (goalVector.Angle() + Constants.HalfPIOffset), 0.05);
		Velocity = new Vector2((float)(-Speed * Math.Sin(-Rotation)), (float)(-Speed * Math.Cos(-Rotation)));
	}

	public override void _PhysicsProcess(double delta)
	{
		MoveAndSlide();
	}
	
	private void SetGunRotationAndPosition()
	{
		_gun.Rotation = (float)Mathf.LerpAngle(_gun.Rotation, _player.Position.AngleToPoint(Position) - Constants.HalfPIOffset, 0.5);
		_gun.Position = Position;
	}
	
	private void HandleAnimations()
	{;
		_hull.Animation = IsIdle ? "default" : "moving";
		_hull.Play();
	}

	private void FindPath()
	{
		_agent.TargetPosition = _player.Position;
	}

	private void OnNavServerReady(Rid rid)
	{
		SetPhysicsProcess(true);
	}
}
