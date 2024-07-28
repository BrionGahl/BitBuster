using System;
using BitBuster.utils;
using Godot;

namespace BitBuster.entity.enemy;

public partial class Panzer : MovingEnemy
{
	private Sprite2D _gun;
	private AnimatedSprite2D _hull;
	private CollisionShape2D _collider;
	private GpuParticles2D _particleDeath;

	private bool _hasDied;
	private bool _animationFinished;
	private int _movementScalar;
	private float _rotationGoal;
	
	public override void _Ready()
	{
		SetPhysicsProcess(false);

		base._Ready();
		_gun = GetNode<Sprite2D>("Gun");
		_hull = GetNode<AnimatedSprite2D>("Hull");
		_collider = GetNode<CollisionShape2D>("Collider");
		_particleDeath = GetNode<GpuParticles2D>("ParticleDeath");
		
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

	public override void OnHealthIsZero()
	{
		_hull.Visible = false;
		_gun.Visible = false;
		_collider.SetDeferred("disabled", true);
		
		StatsComponent.Speed = 0;
		HitboxComponent.SetDeferred("monitorable", false);
		HitboxComponent.SetDeferred("monitoring", false);
		
		_particleDeath.Emitting = true;
		
		DeathAnimationTimer.Start();
		_hasDied = true;
	}

	public override void OnDeathAnimationTimeout()
	{
		_animationFinished = true;
	}

	public override void AttackAction(double delta)
	{
		if (_hasDied)
		{
			if (WeaponComponent.GetChildCount() <= WeaponComponent.BaseChildComponents && _animationFinished)
			{
				Logger.Log.Information(Name + " freed.");
				QueueFree();
			}
			return;
		}
		
		SetGunRotationAndPosition(Mathf.Pi/12);
		if (CanSeePlayer() && RandomNumberGenerator.Randf() > 0.3f)
			WeaponComponent.AttemptShoot(Player.Position.AngleToPoint(Position));
	}

	public override void MoveAction(double delta)
	{
		Vector2 goalVector = (Agent.GetNextPathPosition() - GlobalPosition).Normalized();
		
		// OLD MOVEMENT
		// if (!IsIdle)
		// 	Rotation = Mathf.LerpAngle(Rotation, (goalVector.Angle() + Constants.HalfPiOffset), RotationSpeed / 60 );
		// Velocity = new Vector2((float)(-Speed * Math.Sin(-Rotation)), (float)(-Speed * Math.Cos(-Rotation)));
		
		if (goalVector == Vector2.Zero)
			return;

		Vector2 rotationVector = Vector2.FromAngle(Rotation).Normalized();
		if (rotationVector.DistanceTo(-goalVector.Normalized()) < 0.1f)
		{
			_movementScalar = 1;
			_rotationGoal -= 2 * Mathf.Pi;
		} else if (rotationVector.DistanceTo(goalVector.Normalized()) > 0.6f)
		{
			_movementScalar = 0;
			_rotationGoal = goalVector.Angle();
		}
		else
		{ 
			_movementScalar = 1;
			_rotationGoal = goalVector.Angle();
		}
		
		Rotation = Mathf.LerpAngle(rotationVector.Angle(), _rotationGoal, 0.05f);
		Velocity = goalVector.Normalized() * _movementScalar * Speed;
		MoveAndSlide();
	}

	public override void OnAgentTimeout()
	{
		Agent.TargetPosition = Target == Vector2.Zero ? Player.Position : Target;
	}

	private void OnMapReady(Rid rid)
	{
		SetPhysicsProcess(true);
		AgentTimer.Start();
	}

	
}
