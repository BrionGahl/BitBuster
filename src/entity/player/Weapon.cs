using System.Collections.Generic;
using BitBuster.data;
using BitBuster.projectile;
using BitBuster.utils;
using Godot;

namespace BitBuster.entity.player;

public partial class Weapon : Node2D
{

	[Signal]
	public delegate void BulletCountChangeEventHandler(int count);

	
	[Export] 
	public int BulletCount { get; set; } = 5;

	[Export]
	public float BulletSpeed { get; set; } = 100;

	[Export]
	public float ShootCooldown { get; set; } = 0.33f;

	[Export]
	public int MaxBounces { get; set; } = 1;

	
	
	private Player _parent;
	private PackedScene _bullet;
	private Timer _shootTimer;
	
	private bool _hasShot;
	private bool _canShoot;
	
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_bullet = GD.Load<PackedScene>("res://scenes/subscenes/projectile/bullet.tscn");
		_parent = GetParent<CharacterBody2D>() as Player;
		_shootTimer = GetNode<Timer>("ShootTimer");

		_hasShot = false;
		_canShoot = true;
		
		_shootTimer.Timeout += OnShootTimeout;

		ChildEnteredTree += OnBulletSpawn;
		ChildExitingTree += OnBulletRemove;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GetInput();
		
		// NOTE: Added 1 to offset Timer node child.
		if (_hasShot && _canShoot && BulletCount + 1 - GetChildCount()> 0)
		{
			Logger.Log.Information("Shooting... " + (BulletCount - GetChildCount()) + "/" + BulletCount + ".");
			Shoot();
			_canShoot = false;
			
			_shootTimer.Start(ShootCooldown);
		}
		
	}
	
	private void GetInput() 
	{
		_hasShot = Input.IsActionJustPressed("shoot");
	}

	private void Shoot()
	{
		Bullet bullet = _bullet.Instantiate<CharacterBody2D>() as Bullet;
		bullet.SetTrajectory(_parent.GlobalPosition, GetGlobalMousePosition().AngleToPoint(_parent.GlobalPosition) - Constants.GunSpriteOffset, BulletSpeed, MaxBounces, new AttackData(1.0f, new List<EffectType>()));
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
		EmitSignal(SignalName.BulletCountChange, GetChildren().Count - 1);
	}
}
