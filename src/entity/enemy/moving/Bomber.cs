using BitBuster.component;
using BitBuster.utils;
using BitBuster.world;
using Godot;

namespace BitBuster.entity.enemy.moving;

public partial class Bomber : MovingEnemy
{
	private GlobalEvents _globalEvents;

	private CollisionShape2D _collider;
	private ExplodingComponent _explodingComponent;
	private GpuParticles2D _particleDeath;

	private float _timeTillExplosion;
	private bool _hasExploded;
	
	private bool _hasDied;
	private bool _animationFinished;
	private int _movementScalar;
	private float _rotationGoal;
	
	public override void _Ready()
	{
		SetPhysicsProcess(false);
		base._Ready();
		_globalEvents = GetNode<GlobalEvents>("/root/GlobalEvents");
		
		_collider = GetNode<CollisionShape2D>("Collider");

		_explodingComponent = GetNode<ExplodingComponent>("ExplodingComponent");
		_particleDeath = GetNode<GpuParticles2D>("ParticleDeath");

		_timeTillExplosion = 0f;
		
		NavigationServer2D.MapChanged += OnMapReady;
		AgentTimer.Timeout += OnAgentTimeout;
	}
	
	protected override void OnHealthIsZero()
	{
		SpritesComponent.Visible = false;
		_collider.SetDeferred("disabled", true);

		StatsComponent.Speed = 0;
		HitboxComponent.SetDeferred("monitorable", false);
		HitboxComponent.SetDeferred("monitoring", false);
		
		_particleDeath.Emitting = true;
		
		DeathAnimationTimer.Start();
		_hasDied = true;

	}

	protected override void OnDeathAnimationTimeout()
	{
		_animationFinished = true;
	}

	public override void AttackAction(double delta)
	{
		if (_hasDied)
		{
			if (WeaponComponent.BombsChildren <= 0 && _animationFinished)
			{
				Logger.Log.Information(Name + " freed.");
				QueueFree();
			}
			return;
		}
		
		if (Position.DistanceTo(Player.Position) < 48 && RandomNumberGenerator.Randf() > 0.3f)
		{
			WeaponComponent.AttemptBomb();
		}
	}
	
	public override void MoveAction(double delta)
	{
		Vector2 goalVector = (Agent.GetNextPathPosition() - GlobalPosition).Normalized();
		
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

	protected override void OnAgentTimeout()
	{
		Agent.TargetPosition = Target == Vector2.Zero ? Player.Position : Target;
	}

	private void OnMapReady(Rid rid)
	{
		SetPhysicsProcess(true);
		AgentTimer.Start();
	}
}
