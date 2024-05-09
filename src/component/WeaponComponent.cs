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

	public int BulletCount
	{
		get => StatsComponent.ProjectileCount;
		set => StatsComponent.ProjectileCount = value; 
	}
	
	public bool CanShoot { get; private set; }
	
	private PackedScene _bullet;
	
	public override void _Ready()
	{
		_bullet = GD.Load<PackedScene>("res://scenes/subscenes/projectile/bullet.tscn");
		ShootTimer = GetNode<Timer>("ShootTimer");

		CanShoot = true;
		
		ShootTimer.Timeout += OnShootTimeout;

		ChildEnteredTree += OnBulletSpawn;
		ChildExitingTree += OnBulletRemove;
	}

	public void AttemptShoot(float rotation)
	{
		if (CanShoot && BulletCount + 1 - GetChildCount()> 0)
		{
			Logger.Log.Information("Shooting... " + (BulletCount - GetChildCount()) + "/" + BulletCount + ".");
			
			Shoot(rotation);
			StatsComponent.Speed /= 2;
			CanShoot = false;
			ShootTimer.Start(StatsComponent.ProjectileCooldown);
		}
	}

	private void Shoot(float rotation)
	{
		Bullet bullet = _bullet.Instantiate<CharacterBody2D>() as Bullet;
		bullet.SetTrajectory(GetParent<Node2D>().GlobalPosition, rotation - Constants.HalfPIOffset, StatsComponent.GetAttackData());
		AddChild(bullet);
	}

	private void OnShootTimeout()
	{
		CanShoot = true;
		StatsComponent.Speed *= 2;
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
