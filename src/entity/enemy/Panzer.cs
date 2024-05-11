using System;
using BitBuster.component;
using BitBuster.entity.player;
using BitBuster.state;
using BitBuster.utils;
using Godot;

namespace BitBuster.entity.enemy;

public partial class Panzer : EnemyMoveable
{
	public float Speed
	{
		get => StatsComponent.Speed;
		set => StatsComponent.Speed = value;
	}
	public float RotationSpeed => Speed / 25;
	public bool IsIdle => Velocity.Equals(Vector2.Zero);

	private Sprite2D _gun;
	private AnimatedSprite2D _hull;

	private Vector2 _movementDirection;
	private float _rotationDirection;
	private RandomNumberGenerator _randomNumberGenerator;
	
	public override void _Ready()
	{
		SetPhysicsProcess(false);
	
		base._Ready();	
		_gun = GetNode<Sprite2D>("Gun");
		_hull = GetNode<AnimatedSprite2D>("Hull");

		_randomNumberGenerator = new RandomNumberGenerator();
		_randomNumberGenerator.Randomize();
		
		AgentTimer.Timeout += FindPath;
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
		if (Agent.IsTargetReachable())
			State = EnemyState.Pursue;
	}
	// PURSUE
	private void Pursue()
	{
		if (!Agent.IsTargetReachable())
		{
			EnterIdle();
			return;
		}

		if (Agent.DistanceToTarget() < 64) // Enter Evade
		{
			int x = (int) Math.Floor(Position.X / Constants.RoomSize) * Constants.RoomSize;
			int y = (int) Math.Floor(Position.Y / Constants.RoomSize) * Constants.RoomSize;

			Target = new Vector2(x + Constants.RoomSize - (Player.Position.X % Constants.RoomSize), y + Constants.RoomSize - (Player.Position.Y % Constants.RoomSize));
				
			State = EnemyState.Evade;
		}
		
		SetGunRotationAndPosition();
		if (CanSeePlayer() && _randomNumberGenerator.Randf() > 0.3f)
			WeaponComponent.AttemptShoot(Player.Position.AngleToPoint(Position) + _randomNumberGenerator.RandfRange(-Mathf.Pi / 9, Mathf.Pi / 9));
		
		Vector2 goalVector = (Agent.GetNextPathPosition() - GlobalPosition).Normalized();
		if (!IsIdle)
			Rotation = Mathf.LerpAngle(Rotation, (goalVector.Angle() + Constants.HalfPiOffset), RotationSpeed / 60 );
		Velocity = new Vector2((float)(-Speed * Math.Sin(-Rotation)), (float)(-Speed * Math.Cos(-Rotation)));
		
		MoveAndSlide();
	}
	// EVADE
	private void Evade()
	{
		if (!Agent.IsTargetReachable())
		{
			EnterIdle();
			return;
		}

		if (Agent.DistanceToTarget() < 24)
		{
			State = EnemyState.Pursue;
		}
		
		SetGunRotationAndPosition();
		if (CanSeePlayer() && _randomNumberGenerator.Randf() > 0.3f)
			WeaponComponent.AttemptShoot(Player.Position.AngleToPoint(Position) + _randomNumberGenerator.RandfRange(-Mathf.Pi / 9, Mathf.Pi / 9));

		
		Vector2 goalVector = (Agent.GetNextPathPosition() - GlobalPosition).Normalized();
		if (!IsIdle)
			Rotation = Mathf.LerpAngle(Rotation, (goalVector.Angle() + Constants.HalfPiOffset), RotationSpeed / 60 );
		Velocity = new Vector2((float)(-Speed * Math.Sin(-Rotation)), (float)(-Speed * Math.Cos(-Rotation)));
		
		MoveAndSlide();
	}

	public override void SetGunRotationAndPosition()
	{
		if (CanSeePlayer())
			_gun.Rotation = (float)Mathf.LerpAngle(_gun.Rotation, Player.Position.AngleToPoint(Position) - Constants.HalfPiOffset, 0.5);
		else
			_gun.Rotation = (float)Mathf.LerpAngle(_gun.Rotation, Rotation, 0.1);
		_gun.Position = Position;
	}
	
	public override void HandleAnimations()
	{;
		_hull.Animation = IsIdle ? "default" : "moving";
		_hull.Play();
	}
	
	private void EnterIdle()
	{
		Position = SpawnPosition;
		State = EnemyState.Idle;
		Velocity = Vector2.Zero;
	}

	private void FindPath()
	{
		switch (State)
		{
			case EnemyState.Evade:
				Agent.TargetPosition = Target;
				break;
			default:
				Agent.TargetPosition = Player.Position;
				break;
		}
	}
	
	private void OnMapReady(Rid rid)
	{
		SetPhysicsProcess(true);
	}
}
