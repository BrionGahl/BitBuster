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
	
	public int BulletCount
	{
		get => StatsComponent.ProjectileCount;
		set => StatsComponent.ProjectileCount = value; 
	}
	
	private PackedScene _bullet;
	private Timer _shootTimer;
	
	private bool _hasShot;
	private bool _canShoot;

	public override void _Ready()
	{
		_bullet = GD.Load<PackedScene>("res://scenes/subscenes/projectile/bullet.tscn");
		_shootTimer = GetNode<Timer>("ShootTimer");

		_canShoot = true;
		
		_shootTimer.Timeout += OnShootTimeout;

		ChildEnteredTree += OnBulletSpawn;
		ChildExitingTree += OnBulletRemove;
	}

	public void AttemptShoot()
	{
		if (_canShoot && BulletCount + 1 - GetChildCount()> 0)
		{
			Logger.Log.Information("Shooting... " + (BulletCount - GetChildCount()) + "/" + BulletCount + ".");
			
			Shoot();
			
			_canShoot = false;
			_shootTimer.Start(StatsComponent.ProjectileCooldown);
		}
	}
	
	private void GetInput() 
	{
		_hasShot = Input.IsActionJustPressed("shoot");
	}

	private void Shoot()
	{
		Bullet bullet = _bullet.Instantiate<CharacterBody2D>() as Bullet;
		bullet.SetTrajectory(GetParent<Node2D>().GlobalPosition, GetGlobalMousePosition().AngleToPoint(GetParent<Node2D>().GlobalPosition) - Constants.HalfPIOffset, StatsComponent.GetAttackData());
		AddChild(bullet);
	}

	private void OnShootTimeout()
	{
		_canShoot = true;
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
