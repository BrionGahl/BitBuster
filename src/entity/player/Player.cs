using System;
using BitBuster.component;
using BitBuster.utils;
using Godot;

namespace BitBuster.entity.player;

public partial class Player : CharacterBody2D
{
	[Export]
	private StatsComponent _statsComponent;

	private float Speed
	{
		get => _statsComponent.Speed;
		set => _statsComponent.Speed = value;
	}
	private float RotationSpeed => Speed / 25;
	private bool IsIdle => Velocity.Equals(Vector2.Zero);

	private Sprite2D _gun;
	private AnimatedSprite2D _hull;

	private Vector2 _movementDirection;
	private float _rotationDirection;
	
	public override void _Ready()
	{
		Logger.Log.Information("Loading player...");
		
		_statsComponent ??= GetNode<Node2D>("StatsComponent") as StatsComponent;
		
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
		_hull.Animation = IsIdle ? "default" : "moving";
		_hull.Play();
	}
}
