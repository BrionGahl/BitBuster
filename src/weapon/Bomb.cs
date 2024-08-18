using BitBuster.component;
using BitBuster.data;
using BitBuster.resource;
using BitBuster.world;
using Godot;

namespace BitBuster.weapon;

public partial class Bomb : StaticBody2D
{
	private GlobalEvents _globalEvents;
	private Sprite2D _bombTexture;
	private Timer _deathAnimationTimer;
	private EntityStats _entityStats;
	private HitboxComponent _hitboxComponent;
	private HealthComponent _healthComponent;
	private ExplodingComponent _explodingComponent;
	private AttackData _attackData;

	private float _timeTillExplosion;
	private bool _hasExploded;
	
	[Export] 
	public EntityStats EntityStats
	{
		get => _entityStats;
		set => _entityStats = value;
	}
	
	public override void _Ready()
	{
		_globalEvents = GetNode<GlobalEvents>("/root/GlobalEvents");
		
		_bombTexture = GetNode<Sprite2D>("Sprite2D");
		_deathAnimationTimer = GetNode<Timer>("DeathAnimationTimer");
		
		_explodingComponent = GetNode<ExplodingComponent>("ExplodingComponent");
		_explodingComponent.EntityStats = EntityStats;
		
		_healthComponent = GetNode<HealthComponent>("HealthComponent");
		_healthComponent.EntityStats = EntityStats;

		_hitboxComponent = GetNode<HitboxComponent>("HitboxComponent");
		_hitboxComponent.HealthComponent = _healthComponent;
		
		_timeTillExplosion = 0f;
		Material.Set("shader_parameter/Time", 0f);
		
		_healthComponent.HealthIsZero += OnHealthIsZero;
		_deathAnimationTimer.Timeout += OnDeathAnimationTimeout;
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
		Position = position;
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
