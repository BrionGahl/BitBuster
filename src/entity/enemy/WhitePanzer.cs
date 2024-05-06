using BitBuster.component;
using BitBuster.Component;
using BitBuster.entity.player;
using BitBuster.utils;
using Godot;

namespace BitBuster.entity.enemy;

public partial class WhitePanzer : Enemy
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
		Logger.Log.Information("State: " + State);

		switch (State)
		{
			case EnemyState.Idle:
				if (!_agent.IsTargetReachable())
					return;
				State = EnemyState.Pursue;
				break;
			case EnemyState.Pursue:
				if (!_agent.IsTargetReachable())
				{
					State = EnemyState.Idle;
					Position = _spawnPosition;
					SetGunRotationAndPosition();
					return;
				}
				
				SetGunRotationAndPosition();
				HandleAnimations();
		
				if (!IsIdle)
					Rotation += _rotationDirection * RotationSpeed * (float)delta;

				break;
		}
		
	}

	public override void _PhysicsProcess(double delta)
	{
		Logger.Log.Information("Physics State: " + State);
		switch (State)
		{
			case EnemyState.Idle:
				break;
			case EnemyState.Pursue:
				Vector2 dir = (_agent.GetNextPathPosition() - GlobalPosition).Normalized();
				Velocity = dir * Speed;
		
				MoveAndSlide();
				break;
		}
	}
	
	private void SetGunRotationAndPosition()
	{
		_gun.Rotation = _player.Position.AngleToPoint(Position) - Constants.GunSpriteOffset;
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
