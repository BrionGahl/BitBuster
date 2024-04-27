using System;
using BitBuster.utils;
using Godot;

namespace BitBuster.entity.player;

public partial class Player : CharacterBody2D
{
	private Sprite2D Gun { get; set; }
	private AnimatedSprite2D Hull { get; set; }
	private CollisionShape2D Collider { get; set; }
	
	[Export]
	public float Speed { get; set; } = 300;
	
	[Export]
	public float RotationSpeed { get; set; } = 3f;
	
	private bool IsIdle => Velocity.Equals(Vector2.Zero);

	private Vector2 _movementDirection;
	private float _rotationDirection;
	private const float GunSpriteOffset = (float) Math.PI / 2;
	
	private void GetInput() 
	{
		_rotationDirection = Input.GetAxis("left", "right");
		_movementDirection = Transform.X * Input.GetAxis("down", "up");
	}

	private void SetGunRotationAndPosition()
	{
		Gun.Rotation = GetGlobalMousePosition().AngleToPoint(Position) - GunSpriteOffset;
		Gun.Position = Position;
	}

	private void HandleAnimations()
	{;
		if (IsIdle)
		{
			Hull.Animation = "default";	// maybe make this better / dont hardcode string
		}
		else
		{
			Hull.Animation = "moving";
		}
		
		Hull.Play();

	}

	public override void _Ready()
	{
		Logger.Log.Information("Loading player...");
		
		Gun = GetNode<Sprite2D>("Gun");
		Hull = GetNode<AnimatedSprite2D>("Hull");
		Collider = GetNode<CollisionShape2D>("Collider");
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
