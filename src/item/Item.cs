using BitBuster.weapon;
using BitBuster.world;
using Godot;

namespace BitBuster.item;

public partial class Item: RigidBody2D
{
	// Node Specifics
	private Sprite2D _itemTexture;
	private GpuParticles2D _particles;
	private Label _label;
	
	public Texture2D ItemTexture => _itemTexture.Texture;

	[Export]
	public int ItemId { get; set; }
	[Export]
	public string ItemName { get; set; }
	[Export]
	public string ItemDescription { get; set; }
	[Export]
	public ItemType ItemType { get; set; }


	[Export]
	public int CreditCost { get; set; }
		
	[Export]
	public int AddedBombs { get; set; }
	[Export]
	public int AddedKeyCard { get; set; }
	[Export]
	public int AddedCredit { get; set; }
	[Export]
	public float AddedHealth { get; set; }
	[Export]
	public float AddedOverheal { get; set; }
	
	// Health Related Stats
	[Export]
	public float MaxHealth { get; set; }

	// Bullet Related Stats
	[Export]
	public float ProjectileDamage { get; set; }
	[Export]
	public int ProjectileCount { get; set; }
	[Export]
	public float ProjectileCooldown { get; set; }
	[Export]
	public int ProjectileBounces { get; set; }
	[Export]
	public float ProjectileSpeed { get; set; }
	[Export]
	public EffectType ProjectileDamageType { get; set; }
	[Export]
	public WeaponType ProjectileWeaponType { get; set; }
	[Export]
	public Vector2 ProjectileSizeScalar { get; set; } 
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
	
	
	
	public override void _Ready()
	{
		_itemTexture = GetNode<Sprite2D>("Sprite2D");
		_particles = GetNode<GpuParticles2D>("ParticleItemPickupComponent");
		_label = GetNode<Label>("PriceLabel");
		
		_label.Text = CreditCost <= 0 
			? "" 
			: $"${CreditCost}";

		_particles.Finished += OnParticlesFinished;
	}
	
	public void OnPickup()
	{
		_label.Visible = false;
		_itemTexture.Visible = false;
		_particles.Emitting = true;
	}

	public void SetRandomVelocity()
	{
		RandomNumberGenerator rand = new RandomNumberGenerator();
		LinearVelocity = new Vector2(rand.RandfRange(-75, 75), rand.RandfRange(-75, 75));
	}
	
	private void OnParticlesFinished()
	{
		QueueFree();
	}
}
