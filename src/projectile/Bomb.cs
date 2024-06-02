using BitBuster.component;
using BitBuster.data;
using BitBuster.tiles;
using BitBuster.utils;
using BitBuster.world;
using Godot;

namespace BitBuster.projectile;

public partial class Bomb : StaticBody2D
{
	private Sprite2D _bombTexture;
	private Area2D _hitbox;
	private GpuParticles2D _explodeEmitter;
	private Timer _deathAnimationTimer;
	
	private HitboxComponent _hitboxComponent;
	private HealthComponent _healthComponent;

	private AttackData _attackData;

	private float _timeTillExplosion;
	
	
	public override void _Notification(int what)
	{
		if (what != NotificationSceneInstantiated)
			return;
		
		_bombTexture = GetNode<Sprite2D>("Sprite2D");
		_hitbox = GetNode<Area2D>("Hitbox");
		_explodeEmitter = GetNode<GpuParticles2D>("ExplodeEmitter");
		_deathAnimationTimer = GetNode<Timer>("DeathAnimationTimer");

		
		_hitboxComponent = GetNode<Area2D>("HitboxComponent") as HitboxComponent;
		_healthComponent = GetNode<Node2D>("HealthComponent") as HealthComponent;

		_timeTillExplosion = 2.5f;
		_healthComponent.HealthIsZero += OnHealthIsZero;
		_deathAnimationTimer.Timeout += OnDeathAnimationTimeout;
	}

	public override void _Process(double delta)
	{
		_timeTillExplosion -= (float)delta;
		if (_timeTillExplosion < 0 || _healthComponent.CurrentHealth <= 0)
		{
			_bombTexture.Visible = false;
			_explodeEmitter.Emitting = true;

			if (!_hitbox.Monitoring)
				return;
			
			foreach (var area in _hitbox.GetOverlappingAreas())
			{
				if (area.Equals(_hitboxComponent))
					continue;
				
				if (area is HitboxComponent)
				{
					Logger.Log.Information("Hitbox hit at " + area.Name);

					HitboxComponent hitboxComponent = area as HitboxComponent;
					hitboxComponent.Damage(_attackData);
				}
			}
			
			foreach (var body in _hitbox.GetOverlappingBodies())
			{
				if (body is BreakableWall)
				{
					BreakableWall wall = body as BreakableWall;
					wall.Break();
				}
			}
			
			if (_healthComponent.CurrentHealth > 0)
				_healthComponent.Damage(2f);
			_hitbox.Monitoring = false;
		}
	}

	public void SetPosition(Vector2 position, AttackData attackData)
	{
		GlobalPosition = position;
		_attackData = attackData;
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
