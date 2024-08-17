using BitBuster.entity;
using BitBuster.projectile;
using BitBuster.resource;
using BitBuster.utils;
using BitBuster.world;
using Godot;

namespace BitBuster.component;

public partial class WeaponComponent : Node2D
{

	[Signal]
	public delegate void BulletCountChangeEventHandler(int count);
	
	private EntityStats _entityStats;

	private Node2D Bullets { get; set; }
	private Timer ShootTimer { get; set; }

	private Node2D Bombs { get; set; }
	private Timer BombTimer { get; set; }

	private Node2D ExtraBullets { get; set; }
	
	public int BulletsChildren => Bullets.GetChildCount();
	public int BombsChildren => Bombs.GetChildCount();

	public int BulletCount
	{
		get => _entityStats.ProjectileCount;
		private set => _entityStats.ProjectileCount = value; 
	}
	
	public int BombCount
	{
		get => _entityStats.BombCount;
		set => _entityStats.BombCount = value; 
	}
	
	public bool CanShoot { get; private set; }
	public bool CanBomb { get; private set; }
	
	private PackedScene _bullet;
	private PackedScene _bomb;

	private RandomNumberGenerator _random;
	
	public override void _Ready()
	{
		_bullet = GD.Load<PackedScene>("res://scenes/subscenes/projectile/bullet.tscn");
		_bomb = GD.Load<PackedScene>("res://scenes/subscenes/projectile/bomb.tscn");

		_entityStats = GetParent<Entity>().EntityStats;
		
		Bombs = GetNode<Node2D>("Bombs");
		Bullets = GetNode<Node2D>("Bullets");
		ExtraBullets = GetNode<Node2D>("ExtraBullets");
		
		ShootTimer = GetNode<Timer>("ShootTimer");
		BombTimer = GetNode<Timer>("BombTimer");

		CanShoot = true;
		CanBomb = true;

		_random = new RandomNumberGenerator();
		
		ShootTimer.Timeout += OnShootTimeout;
		BombTimer.Timeout += OnBombTimeout;
		
		Bullets.ChildEnteredTree += OnBulletSpawn;
		Bullets.ChildExitingTree += OnBulletRemove;
	}

	public bool AttemptShoot(float rotation)
	{
		if (!CanShoot || BulletCount - Bullets.GetChildCount() <= 0)
			return false;
		
		if (_entityStats.ProjectileWeaponType.HasFlag(WeaponType.Bi))
		{
			Shoot(rotation + Mathf.Pi + _random.RandfRange(-_entityStats.ProjectileAccuracy, _entityStats.ProjectileAccuracy), ExtraBullets);
		}

		if (_entityStats.ProjectileWeaponType.HasFlag(WeaponType.Tri))
		{
			Shoot(rotation + Mathf.Pi / 9 + _random.RandfRange(-_entityStats.ProjectileAccuracy, _entityStats.ProjectileAccuracy), ExtraBullets);
			Shoot(rotation - Mathf.Pi / 9 + _random.RandfRange(-_entityStats.ProjectileAccuracy, _entityStats.ProjectileAccuracy), ExtraBullets);
		}

		if (_entityStats.ProjectileWeaponType.HasFlag(WeaponType.Quad))
		{
			Shoot(rotation + Mathf.Pi / 2 + _random.RandfRange(-_entityStats.ProjectileAccuracy, _entityStats.ProjectileAccuracy), ExtraBullets);
			Shoot(rotation - Mathf.Pi / 2 + _random.RandfRange(-_entityStats.ProjectileAccuracy, _entityStats.ProjectileAccuracy), ExtraBullets);

		}
		
		if (_entityStats.ProjectileWeaponType.HasFlag(WeaponType.Random))
		{
			Shoot(rotation + _random.RandfRange(0, 2 * Mathf.Pi) + _random.RandfRange(-_entityStats.ProjectileAccuracy, _entityStats.ProjectileAccuracy), ExtraBullets);
		}
	
		Logger.Log.Information("Shooting... " + (BulletCount - Bullets.GetChildCount() - 1) + "/" + BulletCount + ".");

		Shoot(rotation + _random.RandfRange(-_entityStats.ProjectileAccuracy, _entityStats.ProjectileAccuracy), Bullets);
		
		_entityStats.Speed /= 2;
		CanShoot = false;
		ShootTimer.Start(_entityStats.ProjectileCooldown);

		return true;
	}

	private void Shoot(float rotation, Node2D container)
	{
		Bullet bullet = _bullet.Instantiate<CharacterBody2D>() as Bullet;
		bullet.SetTrajectory(GetParent<Node2D>().GlobalPosition, rotation - Constants.HalfPiOffset, 
			_entityStats.GetAttackData(), _entityStats.ProjectileSpeed, _entityStats.ProjectileBounces, 
			_entityStats.ProjectileSizeScalar, _entityStats.ProjectileBulletType, _entityStats.ProjectileBounceType);
		container.AddChild(bullet);
	}

	public bool AttemptBomb(Vector2 position)
	{
		if (!CanBomb || BombCount <= 0)
			return false;
		
		Logger.Log.Information("Bombed...");
		Bomb(position);
			
		CanBomb = false;
		BombTimer.Start();

		return true;
	}

	private void Bomb(Vector2 position)
	{
		Bomb bomb = _bomb.Instantiate<StaticBody2D>() as Bomb;
		bomb.SetPositionAndRadius(position, _entityStats.GetBombAttackData(), _entityStats.BombRadius);
		
		BombCount--;
		_entityStats.EmitStatChangeSignal();
		
		Bombs.AddChild(bomb);
	}

	private void OnShootTimeout()
	{
		CanShoot = true;
		_entityStats.Speed *= 2;
	}
	
	private void OnBombTimeout()
	{
		CanBomb = true;
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
