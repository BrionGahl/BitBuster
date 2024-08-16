using BitBuster.world;
using Godot;

namespace BitBuster.items;

public partial class Item: RigidBody2D
{
	[Export]
	public string ItemName { get; private set; }
	[Export]
	public string ItemDescription { get; private set; }
	[Export]
	public ItemType ItemType { get; private set; }
	
	public Texture2D ItemTexture { get; private set; }
	
	public bool IsUnlocked { get; set; } = true;
	
	[Export]
	public int AddedBombs { get; private set; }
	[Export]
	public int AddedKeyCard { get; private set; }
	[Export]
	public int AddedCredit { get; private set; }
	[Export]
	public float AddedHealth { get; private set; }
	[Export]
	public float AddedOverheal { get; private set; }
	
	// Health Related Stats
	[Export]
	public float MaxHealth { get; private set; }

	// Bullet Related Stats
	[Export]
	public float ProjectileDamage { get; private set; }
	[Export]
	public int ProjectileCount { get; private set; }
	[Export]
	public float ProjectileCooldown { get; private set; }
	[Export]
	public int ProjectileBounces { get; private set; }
	[Export]
	public float ProjectileSpeed { get; private set; }
	[Export]
	public EffectType ProjectileDamageType { get; private set; }
	[Export]
	public WeaponType ProjectileWeaponType { get; private set; }
	[Export]
	public Vector2 ProjectileSizeScalar { get; private set; } 
	[Export]
	public BulletType ProjectileBulletType { get; set; }
	[Export]
	public BounceType ProjectileBounceType { get; set; }
	[Export]
	public TrajectoryType ProjectileTrajectoryType { get; set; }
	[Export]
	public float ProjectileAccuracy { get; set; }
	
	// Bomb Related Stats
	[Export]
	public float BombDamage { get; private set; }
	[Export]
	public EffectType BombDamageType { get; set; }
	[Export]
	public float BombRadius { get; private set; }

	// Control Related Stats
	[Export]
	public float Speed { get; private set; }
	[Export]
	public float ITime { get; private set; }
	[Export]
	public EffectType TrailEffect { get; private set; }

	// Other Stats
	[Export]
	public int Luck { get; private set; }
	[Export]
	public bool OverhealBurst { get; private set; }
	[Export]
	public float OverhealRegen { get; private set; }
	
	// Node Specifics
	public Sprite2D Sprite { get; private set; }
	public Timer AnimationTimer { get; private set; }
	public GpuParticles2D Particles2D { get; private set; }
	
	public override void _Ready()
	{
		
		Sprite = GetNode<Sprite2D>("Sprite2D");
		AnimationTimer = GetNode<Timer>("Timer");
		Particles2D = GetNode<GpuParticles2D>("ParticleItemPickupComponent");

		ItemTexture = Sprite.Texture;
		
		AnimationTimer.Timeout += OnAnimationTimeout;
	}
	
	public void OnPickup()
	{
		Particles2D.Emitting = true;
		
		Sprite.Visible = false;
		
		AnimationTimer.Start();
	}

	public void SetRandomVelocity()
	{
		RandomNumberGenerator rand = new RandomNumberGenerator();
		LinearVelocity = new Vector2(rand.RandfRange(-75, 75), rand.RandfRange(-75, 75));
	}
	
	private void OnAnimationTimeout()
	{
		QueueFree();
	}
}
