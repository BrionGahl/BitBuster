using BitBuster.resource;
using BitBuster.utils;
using BitBuster.weapon;
using BitBuster.world;
using Godot;
using Bomb = BitBuster.weapon.Bomb;
using Bullet = BitBuster.weapon.Bullet;

namespace BitBuster.component;

public partial class WeaponComponent : Node2D
{

	[Signal]
	public delegate void BulletCountChangeEventHandler(int count);

	public EntityStats EntityStats { get; set; }

	private Node2D Bullets { get; set; }
	private Timer ShootTimer { get; set; }

	private Node2D Bombs { get; set; }
	private Timer BombTimer { get; set; }

	private Node2D ExtraBullets { get; set; }
	
	public int BulletsChildren => Bullets.GetChildCount();
	public int BombsChildren => Bombs.GetChildCount();

	public int BulletCount
	{
		get => EntityStats.ProjectileCount;
		private set => EntityStats.ProjectileCount = value; 
	}
	
	public int BombCount
	{
		get => EntityStats.BombCount;
		set => EntityStats.BombCount = value; 
	}
	
	public bool CanShoot => ShootTimer.TimeLeft <= 0;
	public bool CanBomb => BombTimer.TimeLeft <= 0;
	
	private PackedScene _bullet;
	private PackedScene _bomb;

	private RandomNumberGenerator _random;
	
	public override void _Ready()
	{
		_bullet = GD.Load<PackedScene>("res://scenes/subscenes/weapon/bullet.tscn");
		_bomb = GD.Load<PackedScene>("res://scenes/subscenes/weapon/bomb.tscn");
		
		Bombs = GetNode<Node2D>("Bombs");
		Bullets = GetNode<Node2D>("Bullets");
		ExtraBullets = GetNode<Node2D>("ExtraBullets");
		
		ShootTimer = GetNode<Timer>("ShootTimer");
		BombTimer = GetNode<Timer>("BombTimer");

		_random = new RandomNumberGenerator();
		
		ShootTimer.Timeout += OnShootTimeout;
		
		Bullets.ChildEnteredTree += OnBulletSpawn;
		Bullets.ChildExitingTree += OnBulletRemove;
	}

	public bool AttemptShoot(Vector2 sourceLocation, float rotation)
	{
		if (!CanShoot || BulletCount - Bullets.GetChildCount() <= 0)
			return false;
		
		if (EntityStats.ProjectileWeaponType.HasFlag(WeaponType.Bi))
		{
			Shoot(sourceLocation, rotation + Mathf.Pi + _random.RandfRange(-EntityStats.ProjectileAccuracy, EntityStats.ProjectileAccuracy), ExtraBullets);
		}

		if (EntityStats.ProjectileWeaponType.HasFlag(WeaponType.Tri))
		{
			Shoot(sourceLocation, rotation + Mathf.Pi / 9 + _random.RandfRange(-EntityStats.ProjectileAccuracy, EntityStats.ProjectileAccuracy), ExtraBullets);
			Shoot(sourceLocation, rotation - Mathf.Pi / 9 + _random.RandfRange(-EntityStats.ProjectileAccuracy, EntityStats.ProjectileAccuracy), ExtraBullets);
		}

		if (EntityStats.ProjectileWeaponType.HasFlag(WeaponType.Quad))
		{
			Shoot(sourceLocation, rotation + Mathf.Pi / 2 + _random.RandfRange(-EntityStats.ProjectileAccuracy, EntityStats.ProjectileAccuracy), ExtraBullets);
			Shoot(sourceLocation, rotation - Mathf.Pi / 2 + _random.RandfRange(-EntityStats.ProjectileAccuracy, EntityStats.ProjectileAccuracy), ExtraBullets);

		}
		
		if (EntityStats.ProjectileWeaponType.HasFlag(WeaponType.Random))
		{
			Shoot(sourceLocation, rotation + _random.RandfRange(0, 2 * Mathf.Pi) + _random.RandfRange(-EntityStats.ProjectileAccuracy, EntityStats.ProjectileAccuracy), ExtraBullets);
		}
	
		Logger.Log.Information("Shooting... " + (BulletCount - Bullets.GetChildCount() - 1) + "/" + BulletCount + ".");

		Shoot(sourceLocation, rotation + _random.RandfRange(-EntityStats.ProjectileAccuracy, EntityStats.ProjectileAccuracy), Bullets);
		
		EntityStats.Speed /= 2;
		ShootTimer.Start(EntityStats.ProjectileCooldown);

		return true;
	}

	private void Shoot(Vector2 sourceLocation, float rotation, Node2D container)
	{
		Bullet bullet = _bullet.Instantiate<CharacterBody2D>() as Bullet;
		bullet.SetTrajectory(sourceLocation, rotation - Constants.HalfPiOffset, 
			EntityStats.GetAttackData(), EntityStats.ProjectileSpeed, EntityStats.ProjectileBounces, 
			EntityStats.ProjectileSizeScalar, EntityStats.ProjectileBulletType, EntityStats.ProjectileBounceType);
		container.AddChild(bullet);
	}

	public bool AttemptBomb(Vector2 position)
	{
		if (!CanBomb || BombCount <= 0)
			return false;
		
		Logger.Log.Information("Bombed...");
		Bomb(position);
			
		BombTimer.Start();

		return true;
	}

	private void Bomb(Vector2 position)
	{
		Bomb bomb = _bomb.Instantiate<StaticBody2D>() as Bomb;
		bomb.SetPositionAndRadius(position, EntityStats.GetBombAttackData(), EntityStats.BombRadius);
		
		BombCount--;
		EntityStats.EmitStatChangeSignal();
		
		Bombs.AddChild(bomb);
	}

	private void OnShootTimeout()
	{
		EntityStats.Speed *= 2;
	}

	private void OnBulletSpawn(Node node)
	{
		EmitSignal(SignalName.BulletCountChange, Bullets.GetChildren().Count);
	}
	
	private void OnBulletRemove(Node node)
	{
		// OnChildTreeExit signals before child leaves the tree. -1 indicates this.
		EmitSignal(SignalName.BulletCountChange, Bullets.GetChildren().Count - 1);
	}
}
