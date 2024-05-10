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
	private RandomNumberGenerator _randomNumberGenerator;
	
	public override void _Ready()
	{
		SetPhysicsProcess(false);
		
		_gun = GetNode<Sprite2D>("Gun");
		_hull = GetNode<AnimatedSprite2D>("Hull");
		_agent = GetNode<NavigationAgent2D>("Agent");
		_agentTimer = GetNode<Timer>("Agent/Timer");

		_randomNumberGenerator = new RandomNumberGenerator();
		_randomNumberGenerator.Randomize();
		
		_agentTimer.Timeout += FindPath;
		NavigationServer2D.MapChanged += OnMapReady;
	}

	public override void _Process(double delta)
	{
		HandleAnimations();
	}

	public override void _PhysicsProcess(double delta)
	{
		switch (State)
		{
			case EnemyState.Idle:
				Idle();
				break;
			case EnemyState.Pursue:
				Pursue();
				break;
			case EnemyState.Evade:
				Evade();
				break;
		}
	}
	
	
	
	/*
	 * States
	 */ 
	
	// IDLE
	private void Idle()
	{
		if (_agent.IsTargetReachable())
			State = EnemyState.Pursue;
	}
	// PURSUE
	private void Pursue()
	{
		if (!_agent.IsTargetReachable())
		{
			EnterIdle();
			return;
		}

		if (_agent.DistanceToTarget() < 64) // Enter Evade
		{
			int x = (int) Math.Floor(Position.X / Constants.RoomSize) * Constants.RoomSize;
			int y = (int) Math.Floor(Position.Y / Constants.RoomSize) * Constants.RoomSize;

			_target = new Vector2(x + Constants.RoomSize - (_player.Position.X % Constants.RoomSize), y + Constants.RoomSize - (_player.Position.Y % Constants.RoomSize));
				
			State = EnemyState.Evade;
		}
		
		SetGunRotationAndPosition();
		if (CanSeePlayer() && _randomNumberGenerator.Randf() > 0.3f)
			_weaponComponent.AttemptShoot(_player.Position.AngleToPoint(Position) + _randomNumberGenerator.RandfRange(-Mathf.Pi / 9, Mathf.Pi / 9));
		
		Vector2 goalVector = (_agent.GetNextPathPosition() - GlobalPosition).Normalized();
		if (!IsIdle)
			Rotation = Mathf.LerpAngle(Rotation, (goalVector.Angle() + Constants.HalfPiOffset), RotationSpeed / 60 );
		Velocity = new Vector2((float)(-Speed * Math.Sin(-Rotation)), (float)(-Speed * Math.Cos(-Rotation)));
		
		MoveAndSlide();
	}
	// EVADE
	private void Evade()
	{
		if (!_agent.IsTargetReachable())
		{
			EnterIdle();
			return;
		}

		if (_agent.DistanceToTarget() < 24)
		{
			State = EnemyState.Pursue;
		}
		
		SetGunRotationAndPosition();
		if (CanSeePlayer() && _randomNumberGenerator.Randf() > 0.3f)
			_weaponComponent.AttemptShoot(_player.Position.AngleToPoint(Position) + _randomNumberGenerator.RandfRange(-Mathf.Pi / 9, Mathf.Pi / 9));

		
		Vector2 goalVector = (_agent.GetNextPathPosition() - GlobalPosition).Normalized();
		if (!IsIdle)
			Rotation = Mathf.LerpAngle(Rotation, (goalVector.Angle() + Constants.HalfPiOffset), RotationSpeed / 60 );
		Velocity = new Vector2((float)(-Speed * Math.Sin(-Rotation)), (float)(-Speed * Math.Cos(-Rotation)));
		
		MoveAndSlide();
	}

	private void SetGunRotationAndPosition()
	{
		
		if (CanSeePlayer())
			_gun.Rotation = (float)Mathf.LerpAngle(_gun.Rotation, _player.Position.AngleToPoint(Position) - Constants.HalfPiOffset, 0.5);
		else
			_gun.Rotation = (float)Mathf.LerpAngle(_gun.Rotation, Rotation, 0.1);
		_gun.Position = Position;
	}
	
	private void HandleAnimations()
	{;
		_hull.Animation = IsIdle ? "default" : "moving";
		_hull.Play();
	}
	
	private void EnterIdle()
	{
		Position = _spawnPosition;
		State = EnemyState.Idle;
		Velocity = Vector2.Zero;
	}

	private void FindPath()
	{
		switch (State)
		{
			case EnemyState.Evade:
				_agent.TargetPosition = _target;
				break;
			default:
				_agent.TargetPosition = _player.Position;
				break;
		}
	}
	
	private void OnMapReady(Rid rid)
	{
		SetPhysicsProcess(true);
	}
}
