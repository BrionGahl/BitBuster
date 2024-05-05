using BitBuster.component;
using BitBuster.Component;
using BitBuster.utils;
using Godot;

namespace BitBuster.entity.enemy;

public partial class WhitePanzer : CharacterBody2D
{
	[Export]
	private StatsComponent _statsComponent;
	private HealthComponent _healthComponent;
	private HitboxComponent _hitboxComponent;

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
		_gun = GetNode<Sprite2D>("Gun");
		_hull = GetNode<AnimatedSprite2D>("Hull");
	}

	public void LinkNodes() // Make this a parent class
	{
		Logger.Log.Information("Linking Enemy Children...");
		_statsComponent = GetNode<Node2D>("StatsComponent") as StatsComponent;
		_healthComponent = GetNode<Node2D>("HealthComponent") as HealthComponent;
		_hitboxComponent = GetNode<Node2D>("HitboxComponent") as HitboxComponent;

		_healthComponent.LinkNodes();
		_hitboxComponent.LinkNodes();
	}

	public override void _Process(double delta)
	{
		SetGunRotationAndPosition();
	}
	
	private void SetGunRotationAndPosition()
	{
		_gun.Rotation = GetGlobalMousePosition().AngleToPoint(Position) - Constants.GunSpriteOffset;
		_gun.Position = Position;
	}
}
