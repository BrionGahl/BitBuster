using BitBuster.component;
using BitBuster.data;
using BitBuster.entity;
using BitBuster.entity.enemy;
using BitBuster.utils;
using BitBuster.world;
using Godot;

namespace BitBuster.projectile;

public partial class Bullet : CharacterBody2D
{
	private GlobalEvents _globalEvents;
	
	private Sprite2D _bulletTexture;
	private Area2D _hitbox;

	private ExplodingComponent _explodingComponent;
	
	private GpuParticles2D _bounceEmitter;
	private GpuParticles2D _explodeEmitter;

	private Timer _parentIFrameTimer;
	private Timer _selfIFrameTimer;
	private Timer _deathAnimationTimer;

	private AttackData _attackData;
	private int _remainingBounces;
	private float _hueShift;
	
	private BulletType _bulletType;
	private BounceType _bounceType;
	
	public override void _Notification(int what)
	{
		if (what != NotificationSceneInstantiated) 
			return;

		_bulletTexture = GetNode<Sprite2D>("Sprite2D");
		_hitbox = GetNode<Area2D>("Hitbox");

		_explodingComponent = GetNode<ExplodingComponent>("ExplodingComponent");
		
		_bounceEmitter = GetNode<GpuParticles2D>("ParticleBounce");
		_explodeEmitter = GetNode<GpuParticles2D>("ParticleExplode");

		_parentIFrameTimer = GetNode<Timer>("ParentIFrameTimer");
		_selfIFrameTimer = GetNode<Timer>("SelfIFrameTimer");
		_deathAnimationTimer = GetNode<Timer>("DeathAnimationTimer");
		
		_hitbox.AreaEntered += OnAreaEntered;

		_parentIFrameTimer.Timeout += OnParentIFrameTimeout;
		_selfIFrameTimer.Timeout += OnSelfIFrameTimeout;
		_deathAnimationTimer.Timeout += OnDeathAnimationTimeout;
	}

	public override void _Ready()
	{
		_globalEvents = GetNode<GlobalEvents>("/root/GlobalEvents");
		_parentIFrameTimer.Start();
	}
	
	public override void _PhysicsProcess(double delta)
	{
		KinematicCollision2D collision = MoveAndCollide(Velocity * (float)delta);

		if (collision != null && _remainingBounces > 0)
		{
			Velocity = Velocity.Bounce(collision.GetNormal());
			Rotation = Velocity.Angle() - Constants.HalfPiOffset;
			
			_remainingBounces--;
			_bulletTexture.Modulate = Color.FromHsv(_remainingBounces * _hueShift, 1.0f, 1.0f);
			_bounceEmitter.Emitting = true;

			if (_bounceType.HasFlag(BounceType.Compounding))
				_attackData.Damage *= 2;
			
		} else if (collision != null && _remainingBounces <= 0)
		{
			if (_deathAnimationTimer.TimeLeft != 0) 
				return;
			
			PrepForFree();
		}

	}

	public void SetTrajectory(Vector2 position, float rotation, AttackData attackData, float speed, int bounces, Vector2 size, BulletType bulletType, BounceType bounceType)
	{
		GlobalPosition = position;
		GlobalRotation = rotation;

		_attackData = attackData;
		_bulletType = bulletType;
		_bounceType = bounceType;
		
		_remainingBounces = bounces;
		_hueShift = 0.33f / bounces;
		
		_bulletTexture.Modulate = Color.FromHsv(_remainingBounces * _hueShift, 1.0f, 1.0f);

		Scale = new Vector2(size.X, size.Y);
		if (speed > 150)
		{
			GetNode<GpuParticles2D>("ParticleFastTrail").Emitting = true;
		}
		else
		{
			GetNode<GpuParticles2D>("ParticleTrail").Emitting = true;
		}
		
		GetNode<GpuParticles2D>("ParticleCritComponent").Emitting = _attackData.IsCrit;
		
		Velocity = new Vector2(0, -speed).Rotated(GlobalRotation);
	}

	public void PrepForFree()
	{
		_bulletTexture.Visible = false;
		
		_hitbox.SetDeferred("monitoring", false);
		_hitbox.SetDeferred("monitorable", false);
		
		_explodeEmitter.Emitting = true;
		_deathAnimationTimer.Start();
	}

	private void OnAreaEntered(Area2D area)
	{
		if (area.IsInGroup(Groups.GroupItem))
			return;

		if (area.IsInGroup(Groups.GroupBulletNoPass))
		{
			PrepForFree();
			return;
		}
		
		if (area is HitboxComponent hitboxComponent)
		{
			Logger.Log.Information("Hitbox hit at " + hitboxComponent.Name);
			if (hitboxComponent.GetParent() is Enemy && _bulletType.HasFlag(BulletType.Invulnerable))
				return;
			hitboxComponent.Damage(_attackData);
		}

		if (_bulletType.HasFlag(BulletType.Exploding))
		{
			_explodingComponent.Explode(new AttackData(1f, EffectType.Normal, _attackData.SourceType, false));
		}
		
		if (_bulletType.HasFlag(BulletType.Piercing))
		{
			_remainingBounces--;
			if (_remainingBounces < 0)
			{
				PrepForFree();
				return;
			}
			_bulletTexture.Modulate = Color.FromHsv(_remainingBounces * _hueShift, 1.0f, 1.0f);
			return;
		}
		
		if (_bulletType.HasFlag(BulletType.Invulnerable))
			return;
		
		
			
		PrepForFree();
	}

	private void OnParentIFrameTimeout()
	{

		if (_attackData.SourceType == SourceType.Enemy)
			_hitbox.SetCollisionMaskValue((int)BBCollisionLayer.Player, true);
		else
			_hitbox.SetCollisionMaskValue((int)BBCollisionLayer.Enemy, true);
		
		_hitbox.SetCollisionMaskValue((int)BBCollisionLayer.Projectile, true);
		_hitbox.SetCollisionMaskValue((int)BBCollisionLayer.Item, true);
		
		_selfIFrameTimer.Start();
	}

	private void OnSelfIFrameTimeout()
	{
		if (_attackData.SourceType == SourceType.Enemy)
			_hitbox.SetCollisionMaskValue((int)BBCollisionLayer.Enemy, true);
		else
			_hitbox.SetCollisionMaskValue((int)BBCollisionLayer.Player, true);
	}
	
	private void OnDeathAnimationTimeout()
	{
		QueueFree();
	}
}
