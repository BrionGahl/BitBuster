using System;
using BitBuster.utils;
using Godot;

namespace BitBuster.projectile;

public partial class Bullet : CharacterBody2D
{

	private Sprite2D _bulletTexture;
	private Area2D _hitbox;
	
	private int _remainingBounces;
	private float _parentInvincibilityFrames;
	
	public override void _Notification(int what)
	{
		if (what != NotificationSceneInstantiated) 
			return;

		_bulletTexture = GetNode<Sprite2D>("Sprite2D");
		_hitbox = GetNode<Area2D>("Hitbox");
		
		TopLevel = true;
		
		_hitbox.AreaEntered += OnAreaEntered;
	}
	
	public override void _Process(double delta)
	{
		// TODO: Get color change on remaining bounces.
		// if (_remainingBounces > 0)
		// 	Modulate = Color.FromHsv(0.0f + (_remainingBounces / 4), 0.5f, 0.4f);
	}

	public override void _PhysicsProcess(double delta)
	{
		KinematicCollision2D collision = MoveAndCollide(Velocity * (float)delta);

		if (collision != null && _remainingBounces > 0)
		{
			Velocity = Velocity.Bounce(collision.GetNormal());
			Rotation = Velocity.Angle() - Constants.GunSpriteOffset;

			_remainingBounces--;
		} else if (collision != null && _remainingBounces == 0)
		{
			QueueFree();
		}
	}

	public void SetTrajectory(Vector2 position, float rotation, float speed, int maxBounces)
	{
		GlobalPosition = position;
		GlobalRotation = rotation;
		
		_remainingBounces = maxBounces;
		
		Velocity = new Vector2(0, -speed).Rotated(GlobalRotation);
	}

	private void OnAreaEntered(Area2D area)
	{
		Logger.Log.Information("Hitbox hit at " + area);
		
		
	}
}
