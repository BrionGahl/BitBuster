using System;
using BitBuster.component;
using BitBuster.data;
using BitBuster.utils;
using BitBuster.world;
using Godot;

namespace BitBuster.projectile;

public partial class Bullet : CharacterBody2D
{
	private Sprite2D _bulletTexture;
	private Area2D _hitbox;
	private VisibleOnScreenNotifier2D _screenNotifier;

	private GpuParticles2D _bounceEmitter;
	private GpuParticles2D _explodeEmitter;

	private Timer _parentIFrameTimer;
	private Timer _deathAnimationTimer;

	private AttackData _attackData;
	private int _remainingBounces;
	private float _hueShift;
	
	public override void _Notification(int what)
	{
		if (what != NotificationSceneInstantiated) 
			return;

		_bulletTexture = GetNode<Sprite2D>("Sprite2D");
		_hitbox = GetNode<Area2D>("Hitbox");

		_bounceEmitter = GetNode<GpuParticles2D>("ParticleBounce");
		_explodeEmitter = GetNode<GpuParticles2D>("ParticleExplode");

		_parentIFrameTimer = GetNode<Timer>("ParentIFrameTimer");
		_deathAnimationTimer = GetNode<Timer>("DeathAnimationTimer");
		
		TopLevel = true;
		
		_hitbox.AreaEntered += OnAreaEntered;

		_parentIFrameTimer.Timeout += OnParentIFrameTimeout;
		_deathAnimationTimer.Timeout += OnDeathAnimationTimeout;
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
		} else if (collision != null && _remainingBounces == 0)
		{
			if (_deathAnimationTimer.TimeLeft != 0) 
				return;
			
			PrepForFree();
			_deathAnimationTimer.Start();
		}

	}

	public void SetTrajectory(Vector2 position, float rotation, AttackData attackData)
	{
		GlobalPosition = position;
		GlobalRotation = rotation;
		
		_remainingBounces = attackData.Bounces;
		_hueShift = 0.33f / attackData.Bounces;
		
		_bulletTexture.Modulate = Color.FromHsv(_remainingBounces * _hueShift, 1.0f, 1.0f);

		_attackData = attackData;
		
		GetNode<GpuParticles2D>("ParticleTrail").Emitting = true;
		if (attackData.Speed > 150)
		{
			GetNode<GpuParticles2D>("ParticleTrail").Emitting = false;
			GetNode<GpuParticles2D>("ParticleFastTrail").Emitting = true;
			
		}
			
		Velocity = new Vector2(0, -attackData.Speed).Rotated(GlobalRotation);
	}

	private void PrepForFree()
	{
		_bulletTexture.Visible = false;
		
		_hitbox.SetDeferred("monitoring", false);
		_hitbox.SetDeferred("monitorable", false);
		
		_explodeEmitter.Emitting = true;
	}

	private void OnAreaEntered(Area2D area)
	{
		if (area is HitboxComponent)
		{
			Logger.Log.Information("Hitbox hit at " + area.Name);

			HitboxComponent hitboxComponent = area as HitboxComponent;

			hitboxComponent.Damage(_attackData);
		}
		
		PrepForFree();
		_deathAnimationTimer.Start();
		
	}

	private void OnParentIFrameTimeout()
	{
		_hitbox.SetCollisionMaskValue((int)BBCollisionLayer.Player, true);
		_hitbox.SetCollisionMaskValue((int)BBCollisionLayer.Enemy, true);
		_hitbox.SetCollisionMaskValue((int)BBCollisionLayer.Projectile, true);
		SetCollisionMaskValue((int)BBCollisionLayer.Projectile, true);
	}
	
	private void OnDeathAnimationTimeout()
	{
		Logger.Log.Information("Freeing bullet...");
		QueueFree();
	}
}
