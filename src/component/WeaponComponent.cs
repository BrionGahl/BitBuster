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
	
	public Timer ShootTimer { get; private set; }
	public Timer BombTimer { get; private set; }
	
	public int BulletCount
	{
		get => StatsComponent.ProjectileCount;
		set => StatsComponent.ProjectileCount = value; 
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
	
	public override void _Ready()
	{
		_bullet = GD.Load<PackedScene>("res://scenes/subscenes/projectile/bullet.tscn");
		_bomb = GD.Load<PackedScene>("res://scenes/subscenes/projectile/bomb.tscn");
		
		ShootTimer = GetNode<Timer>("ShootTimer");
		BombTimer = GetNode<Timer>("BombTimer");

		CanShoot = true;
		CanBomb = true;
		
		ShootTimer.Timeout += OnShootTimeout;
		BombTimer.Timeout += OnBombTimeout;
		
		ChildEnteredTree += OnBulletSpawn;
		ChildExitingTree += OnBulletRemove;
	}

	public void AttemptShoot(float rotation)
	{
		switch (StatsComponent.ProjectileWeaponType)
		{
			case (WeaponType.Normal):
				if (CanShoot && BulletCount + 2 - GetChildCount() > 0)
				{
					Logger.Log.Information("Shooting... " + (BulletCount + 2 - GetChildCount()) + "/" + BulletCount + ".");
			
					Shoot(rotation);
					
					StatsComponent.Speed /= 2;
					CanShoot = false;
					ShootTimer.Start(StatsComponent.ProjectileCooldown);
				}
				break;
			case (WeaponType.Tri):
				if (CanShoot && BulletCount + 2 - GetChildCount() > 2)
				{
					Logger.Log.Information("Shooting... " + (BulletCount + 2 - GetChildCount()) + "/" + BulletCount + ".");
			
					Shoot(rotation + Mathf.Pi / 9);
					Shoot(rotation);
					Shoot(rotation - Mathf.Pi / 9);

					StatsComponent.Speed /= 2;
					CanShoot = false;
					ShootTimer.Start(StatsComponent.ProjectileCooldown);
				}
				break;
		}
		
	}

	private void Shoot(float rotation)
	{
		Bullet bullet = _bullet.Instantiate<CharacterBody2D>() as Bullet;
		bullet.SetTrajectory(GetParent<Node2D>().GlobalPosition, rotation - Constants.HalfPiOffset, StatsComponent.GetAttackData());
		AddChild(bullet);
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
		bomb.SetPosition(GetParent<Node2D>().GlobalPosition, StatsComponent.GetBombAttackData());
		BombCount--;
		GetNode("/root").AddChild(bomb);
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
		EmitSignal(SignalName.BulletCountChange, GetChildren().Count);
	}
	
	private void OnBulletRemove(Node node)
	{
		// OnChildTreeExit signals before child leaves the tree. -1 indicates this.
		EmitSignal(SignalName.BulletCountChange, GetChildren().Count - 1);
	}
}
