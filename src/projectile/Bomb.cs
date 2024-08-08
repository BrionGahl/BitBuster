using BitBuster.component;
using BitBuster.data;
using BitBuster.resource;
using BitBuster.world;
using Godot;

namespace BitBuster.projectile;

public partial class Bomb : StaticBody2D
{
	private GlobalEvents _globalEvents;
	
	private Sprite2D _bombTexture;
	
	private Timer _deathAnimationTimer;
	
	[Export]
	public EntityStats EntityStats;
	
	private HitboxComponent _hitboxComponent;
	private HealthComponent _healthComponent;
	private ExplodingComponent _explodingComponent;

	private AttackData _attackData;

	private float _timeTillExplosion;
	private bool _hasExploded;
	
	public override void _Notification(int what)
	{
		if (what != NotificationSceneInstantiated)
			return;
		
		_bombTexture = GetNode<Sprite2D>("Sprite2D");
		_deathAnimationTimer = GetNode<Timer>("DeathAnimationTimer");
		
		_explodingComponent = GetNode<ExplodingComponent>("ExplodingComponent");
		_hitboxComponent = GetNode<Area2D>("HitboxComponent") as HitboxComponent;
		_healthComponent = GetNode<Node2D>("HealthComponent") as HealthComponent;
		
		_timeTillExplosion = 0f;
		Material.Set("shader_parameter/Time", 0f);
		
		_healthComponent.HealthIsZero += OnHealthIsZero;
		_deathAnimationTimer.Timeout += OnDeathAnimationTimeout;
	}
	
	public override void _Ready()
	{
		_globalEvents = GetNode<GlobalEvents>("/root/GlobalEvents");
	}

	public override void _Process(double delta)
	{
		_timeTillExplosion += (float)delta;
		Material.Set("shader_parameter/time", _timeTillExplosion);

		if ((_timeTillExplosion >= 2.5f || _healthComponent.CurrentHealth <= 0) && !_hasExploded)
		{
			_hasExploded = true;

			_healthComponent.Damage(_healthComponent.CurrentHealth);
			_bombTexture.Visible = false;
			_explodingComponent.Explode(_attackData);
		}
	}

	public void SetPositionAndRadius(Vector2 position, AttackData attackData, float radius)
	{
		GlobalPosition = position;
		_attackData = attackData;
		EntityStats.BombRadius = radius;
		
		GetNode<GpuParticles2D>("ParticleCritComponent").Emitting = _attackData.IsCrit;
	}

	private void OnHealthIsZero()
	{
		_hitboxComponent.SetDeferred("monitorable", false);
		_hitboxComponent.SetDeferred("monitoring", false);
		
		_deathAnimationTimer.Start();
	}

	private void OnDeathAnimationTimeout()
	{
		QueueFree();
	}
}
