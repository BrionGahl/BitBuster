using System.Collections.Generic;
using BitBuster.component;
using BitBuster.data;
using BitBuster.entity.player;
using BitBuster.projectile;
using BitBuster.utils;
using BitBuster.world;
using Godot;

namespace BitBuster.component;

public partial class WeaponComponent : Node2D
{

	[Signal]
	public delegate void BulletCountChangeEventHandler(int count);
	
	[Export]
	public StatsComponent StatsComponent;

	private Node2D Bullets { get; set; }
	private Timer ShootTimer { get; set; }

	private Node2D Bombs { get; set; }
	private Timer BombTimer { get; set; }

	private Node2D ExtraBullets { get; set; }
	
	public int BulletsChildren => Bullets.GetChildCount();

	public int BulletCount
	{
		get => StatsComponent.ProjectileCount;
		private set => StatsComponent.ProjectileCount = value; 
	}
	
	public int BombCount
	{
		get => StatsComponent.BombCount;
		set => StatsComponent.BombCount = value; 
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

	public void AttemptShoot(float rotation)
	{
		if (!CanShoot || BulletCount - Bullets.GetChildCount() <= 0)
			return;
		
		if (StatsComponent.ProjectileWeaponType.HasFlag(WeaponType.Bi))
		{
			Shoot(rotation + Mathf.Pi + _random.RandfRange(-StatsComponent.ProjectileAccuracy, StatsComponent.ProjectileAccuracy), ExtraBullets);
		}

		if (StatsComponent.ProjectileWeaponType.HasFlag(WeaponType.Tri))
		{
			Shoot(rotation + Mathf.Pi / 9 + _random.RandfRange(-StatsComponent.ProjectileAccuracy, StatsComponent.ProjectileAccuracy), ExtraBullets);
			Shoot(rotation - Mathf.Pi / 9 + _random.RandfRange(-StatsComponent.ProjectileAccuracy, StatsComponent.ProjectileAccuracy), ExtraBullets);
		}

		if (StatsComponent.ProjectileWeaponType.HasFlag(WeaponType.Quad))
		{
			Shoot(rotation + Mathf.Pi / 2 + _random.RandfRange(-StatsComponent.ProjectileAccuracy, StatsComponent.ProjectileAccuracy), ExtraBullets);
			Shoot(rotation - Mathf.Pi / 2 + _random.RandfRange(-StatsComponent.ProjectileAccuracy, StatsComponent.ProjectileAccuracy), ExtraBullets);

		}
		
		if (StatsComponent.ProjectileWeaponType.HasFlag(WeaponType.Random))
		{
			Shoot(rotation + _random.RandfRange(0, 2 * Mathf.Pi) + _random.RandfRange(-StatsComponent.ProjectileAccuracy, StatsComponent.ProjectileAccuracy), ExtraBullets);
		}
	
		Logger.Log.Information("Shooting... " + (BulletCount - Bullets.GetChildCount() - 1) + "/" + BulletCount + ".");

		Shoot(rotation + _random.RandfRange(-StatsComponent.ProjectileAccuracy, StatsComponent.ProjectileAccuracy), Bullets);
		
		StatsComponent.Speed /= 2;
		CanShoot = false;
		ShootTimer.Start(StatsComponent.ProjectileCooldown);
	}

	private void Shoot(float rotation, Node2D container)
	{
		Bullet bullet = _bullet.Instantiate<CharacterBody2D>() as Bullet;
		bullet.SetTrajectory(GetParent<Node2D>().GlobalPosition, rotation - Constants.HalfPiOffset, 
			StatsComponent.GetAttackData(), StatsComponent.ProjectileSpeed, StatsComponent.ProjectileBounces, 
			StatsComponent.ProjectileSizeScalar, StatsComponent.ProjectileBulletType);
		container.AddChild(bullet);
	}

	public void AttemptBomb()
	{
		if (CanBomb && BombCount > 0)
		{
			Logger.Log.Information("Bombed...");
			Bomb();
			
			CanBomb = false;
			BombTimer.Start();
		} 
	}

	private void Bomb()
	{
		Bomb bomb = _bomb.Instantiate<StaticBody2D>() as Bomb;
		bomb.SetPositionAndRadius(GetParent<Node2D>().GlobalPosition, StatsComponent.GetBombAttackData(), StatsComponent.BombRadius);
		
		BombCount--;
		StatsComponent.EmitStatChangeSignal();
		
		Bombs.AddChild(bomb);
	}

	private void OnShootTimeout()
	{
		CanShoot = true;
		StatsComponent.Speed *= 2;
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
