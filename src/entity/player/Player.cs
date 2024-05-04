using System;
using BitBuster.utils;
using Godot;

namespace BitBuster.entity.player;

public partial class Player : CharacterBody2D
{
	[Export]
	public float Speed { get; set; } = 300;
	
	[Export]
	public float RotationSpeed { get; set; } = 3f;
	
	private bool IsIdle => Velocity.Equals(Vector2.Zero);

	private Sprite2D _gun;
	private AnimatedSprite2D _hull;

	private Vector2 _movementDirection;
	private float _rotationDirection;
	
	private void GetInput() 
	{
		_rotationDirection = Input.GetAxis("left", "right");
		_movementDirection = Transform.X * Input.GetAxis("down", "up");
	}

	private void SetGunRotationAndPosition()
	{
		_gun.Rotation = GetGlobalMousePosition().AngleToPoint(Position) - Constants.GunSpriteOffset;
		_gun.Position = Position;
	}

	private void HandleAnimations()
	{;
		if (IsIdle)
		{
			_hull.Animation = "default";	// maybe make this better / dont hardcode string
		}
		else
		{
			_hull.Animation = "moving";
		}
		
		_hull.Play();

	}

	public override void _Ready()
	{
		Logger.Log.Information("Loading player...");
		
		_gun = GetNode<Sprite2D>("Gun");
		_hull = GetNode<AnimatedSprite2D>("Hull");
	}
	
	public override void _Process(double delta)
	{
		GetInput();
		SetGunRotationAndPosition();
		
		HandleAnimations();
		
		if (!IsIdle)
			Rotation += _rotationDirection * RotationSpeed * (float)delta;
		
		Velocity = _movementDirection * Speed;

	}
	
	public override void _PhysicsProcess(double delta)
	{
		MoveAndSlide();
	}
}
