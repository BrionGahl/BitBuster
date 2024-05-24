using BitBuster.component;
using BitBuster.data;
using BitBuster.utils;
using Godot;

namespace BitBuster.projectile;

public partial class Bomb : StaticBody2D
{
	private Sprite2D _bombTexture;
	private Area2D _hitbox;
	private GpuParticles2D _explodeEmitter;

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

		_hitboxComponent = GetNode<Area2D>("HitboxComponent") as HitboxComponent;
		_healthComponent = GetNode<Node2D>("HealthComponent") as HealthComponent;

		_timeTillExplosion = 2.5f;
	}

	public override void _Process(double delta)
	{
		_timeTillExplosion -= (float)delta;
		// TODO: use a similar check on current health for things that need to be gone after death.
		if (_timeTillExplosion < 0 || _healthComponent.CurrentHealth <= 0)
		{
			_explodeEmitter.Emitting = true;

			if (!_hitbox.Monitoring)
				return;
			
			foreach (var area in _hitbox.GetOverlappingAreas())
			{
				if (area is HitboxComponent)
				{
					Logger.Log.Information("Hitbox hit at " + area.Name);

					HitboxComponent hitboxComponent = area as HitboxComponent;
					hitboxComponent.Damage(_attackData);
				}
			}
			_hitbox.Monitoring = false;
			PrepForFree();
		}
	}

	public void SetPosition(Vector2 position, AttackData attackData)
	{
		GlobalPosition = position;
		_attackData = attackData;
	}
	
	private void PrepForFree()
	{
		_bombTexture.Visible = false;
		_hitboxComponent.Monitorable = false;
		_hitboxComponent.Monitoring = false;

	}
}
