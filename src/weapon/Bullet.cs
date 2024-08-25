using BitBuster.component;
using BitBuster.data;
using BitBuster.entity.enemy;
using BitBuster.utils;
using BitBuster.world;
using Godot;

namespace BitBuster.weapon;

public partial class Bullet : CharacterBody2D
{
	private GlobalEvents _globalEvents;

	private RandomNumberGenerator _random;
	
	private Sprite2D _bulletTexture;
	private Sprite2D _bulletInvulnTexture;
	private Area2D _hitbox;
	private CollisionShape2D _hitboxCollision;
	private CollisionShape2D _collision;
	private RemoteTransform2D _remoteShieldTransform;

	private ExplodingComponent _explodingComponent;
	
	private GpuParticles2D _bounceEmitter;
	private GpuParticles2D _explodeEmitter;
	private GpuParticles2D _activeTrail;

	private Timer _parentIFrameTimer;
	private Timer _selfIFrameTimer;

	private int _remainingBounces;
	private float _hueShift;
	
	private BulletType _bulletType;
	private BounceType _bounceType;
	
	public AttackData AttackData { get; private set; }
	
	public override void _Notification(int what)
	{
		if (what != NotificationSceneInstantiated) 
			return;

		_random = new RandomNumberGenerator();
		
		_bulletTexture = GetNode<Sprite2D>("Sprite2D");
		_bulletInvulnTexture = GetNode<Sprite2D>("BulletInvulnerableSprite");
		_hitbox = GetNode<Area2D>("Hitbox");
		_hitboxCollision = GetNode<CollisionShape2D>("Hitbox/AreaCollider");
		_remoteShieldTransform = GetNode<RemoteTransform2D>("RemoteTransform2D");

		_collision = GetNode<CollisionShape2D>("CollisionShape2D");
		
		_explodingComponent = GetNode<ExplodingComponent>("ExplodingComponent");
		
		_bounceEmitter = GetNode<GpuParticles2D>("ParticleBounce");
		_explodeEmitter = GetNode<GpuParticles2D>("ParticleExplode");

		_parentIFrameTimer = GetNode<Timer>("ParentIFrameTimer");
		_selfIFrameTimer = GetNode<Timer>("SelfIFrameTimer");
		
		_hitbox.AreaEntered += OnAreaEntered;

		_parentIFrameTimer.Timeout += OnParentIFrameTimeout;
		_selfIFrameTimer.Timeout += OnSelfIFrameTimeout;
		_explodeEmitter.Finished += OnExplodeFinished;
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
				AttackData.Damage *= 2;

			if (_bounceType.HasFlag(BounceType.Accelerating))
				Velocity *= 1.33f;
			
			if (_bounceType.HasFlag(BounceType.Changing))
				Velocity = Velocity.Bounce(collision.GetNormal() + new Vector2(_random.RandfRange(-Mathf.Pi, Mathf.Pi), _random.RandfRange(-Mathf.Pi, Mathf.Pi)));

		} else if (collision != null && _remainingBounces <= 0)
		{
			if (_explodeEmitter.Emitting) 
				return;
			
			PrepForFree();
		}

	}

	public void SetTrajectory(Vector2 position, float rotation, AttackData attackData, float speed, int bounces, Vector2 size, BulletType bulletType, BounceType bounceType)
	{
		GlobalPosition = position;
		GlobalRotation = rotation;

		AttackData = attackData;
		_bulletType = bulletType;
		_bounceType = bounceType;
		
		_remainingBounces = bounces;
		_hueShift = 0.33f / bounces;

		_bulletInvulnTexture.Visible = _bulletType.HasFlag(BulletType.Invulnerable);
		
		_bulletTexture.Modulate = Color.FromHsv(_remainingBounces * _hueShift, 1.0f, 1.0f);

		_bulletTexture.Scale = new Vector2(size.X, size.Y);
		_collision.Scale = new Vector2(size.X, size.Y);
		((RectangleShape2D)_hitboxCollision.Shape).Size = new Vector2(2 * size.X + 2, 4 * size.Y + 2);
		_remoteShieldTransform.Scale = new Vector2(2f * size.X, 1.5f * size.Y);
		
		_activeTrail = speed > 150
			? GetNode<GpuParticles2D>("ParticleFastTrail")
			: GetNode<GpuParticles2D>("ParticleTrail");
		_activeTrail.Emitting = true;
		
		GetNode<GpuParticles2D>("ParticleCritComponent").Emitting = AttackData.IsCrit;
		
		Velocity = new Vector2(0, -speed).Rotated(GlobalRotation);
	}

	private void PrepForFree()
	{
		_bulletTexture.Visible = false;
		_activeTrail.Emitting = false;
		_bounceEmitter.Visible = false;
		_bulletInvulnTexture.Visible = false;
		
		_hitbox.SetDeferred("monitoring", false);
		_hitbox.SetDeferred("monitorable", false);
		
		_explodeEmitter.Emitting = true;
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
			if (hitboxComponent.Source is SourceType.Enemy && _bulletType.HasFlag(BulletType.Invulnerable))
				return;
			hitboxComponent.Damage(AttackData);
		}

		if (_bulletType.HasFlag(BulletType.Exploding))
		{
			_explodingComponent.Explode(new AttackData(1f, AttackData.Effects, AttackData.SourceType, false));
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

		if (AttackData.SourceType == SourceType.Enemy)
			_hitbox.SetCollisionMaskValue((int)BbCollisionLayer.Player, true);
		else
			_hitbox.SetCollisionMaskValue((int)BbCollisionLayer.Enemy, true);
		
		_hitbox.SetCollisionMaskValue((int)BbCollisionLayer.Projectile, true);
		_hitbox.SetCollisionMaskValue((int)BbCollisionLayer.Item, true);
		
		_selfIFrameTimer.Start();
	}

	private void OnSelfIFrameTimeout()
	{
		if (AttackData.SourceType == SourceType.Enemy)
			_hitbox.SetCollisionMaskValue((int)BbCollisionLayer.Enemy, true);
		else
			_hitbox.SetCollisionMaskValue((int)BbCollisionLayer.Player, true);
	}
	
	private void OnExplodeFinished()
	{
		QueueFree();
	}
}
