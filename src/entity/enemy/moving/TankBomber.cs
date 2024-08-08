using BitBuster.component;
using BitBuster.utils;
using BitBuster.world;
using Godot;

namespace BitBuster.entity.enemy.moving;

public partial class TankBomber : MovingEnemy
{
	private GlobalEvents _globalEvents;

	private CollisionShape2D _collider;
	private ExplodingComponent _explodingComponent;
	private GpuParticles2D _particleDeath;

	private float _timeTillExplosion;
	private bool _hasExploded;
	
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

		EntityStats.Speed = 0;
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
		AttemptToFree();
		if (_hasDied)
			return;

		if (Position.DistanceTo(Player.Position) < 48 && RandomNumberGenerator.Randf() > 0.3f)
		{
			WeaponComponent.AttemptBomb(Position);
		}
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
