using BitBuster.projectile;
using BitBuster.utils;
using Godot;

namespace BitBuster.entity.player;

public partial class Weapon : Node2D
{

	[Export] 
	public int BulletCount { get; set; } = 5;

	[Export]
	public float BulletSpeed { get; set; } = 100;

	[Export]
	public float ShootCooldown { get; set; } = 1;

	[Export]
	public int MaxBounces { get; set; } = 3;

	
	
	private Player _parent;
	private PackedScene _bullet;
	private bool _hasShot;
	private float _shootCooldown;
	
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_bullet = GD.Load<PackedScene>("res://scenes/subscenes/projectile/bullet.tscn");
		_parent = GetParent<CharacterBody2D>() as Player;
		
		_hasShot = false;
		_shootCooldown = ShootCooldown;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GetInput();

		if (_hasShot && _shootCooldown <= 0 && BulletCount - GetChildCount() > 0)
		{
			Logger.Log.Information("Shooting... " + (BulletCount - GetChildCount()) + "/" + BulletCount + ".");
			Shoot();
			_shootCooldown = ShootCooldown;
		}

		if (_shootCooldown > 0)
			_shootCooldown -= (float)delta;
	}
	
	private void GetInput() 
	{
		_hasShot = Input.IsActionJustPressed("shoot");
	}

	private void Shoot()
	{
		Bullet bullet = _bullet.Instantiate<CharacterBody2D>() as Bullet;
		bullet.SetTrajectory(_parent.GlobalPosition, GetGlobalMousePosition().AngleToPoint(_parent.GlobalPosition) - Constants.GunSpriteOffset, BulletSpeed, MaxBounces);
		AddChild(bullet);
	}
}
