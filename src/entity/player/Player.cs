using BitBuster.component;
using BitBuster.utils;
using Godot;
using Serilog;

namespace BitBuster.entity.player;

public partial class Player : CharacterBody2D
{
	[Export]
	private StatsComponent _statsComponent;

	[Export] 
	private WeaponComponent _weaponComponent;

	[Export] 
	private HealthComponent _healthComponent;
	
	private float Speed
	{
		get => _statsComponent.Speed;
		set => _statsComponent.Speed = value;
	}
	private float RotationSpeed => Speed / 25;
	private bool IsIdle => Velocity.Equals(Vector2.Zero);

	private Sprite2D _gun;
	private AnimatedSprite2D _hull;
	private AnimationPlayer _animationPlayer;

	// OLD MOVEMENT
	// private Vector2 _movementDirection;
	// private float _rotationDirection;
	
	// NEW MOVEMENT
	private Vector2 _movementDirection;
	private float _rotationGoal;
	private int _movementScalar;
	
	private bool _hasShot;
	private bool _hasBombed;
	
	public override void _Ready()
	{
		Logger.Log.Information("Loading player...");
		
		_gun = GetNode<Sprite2D>("Gun");
		_hull = GetNode<AnimatedSprite2D>("Hull");
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

		_healthComponent.HealthChange += OnHealthChange;
	}
	
	public override void _Process(double delta)
	{
		GetInput();
		SetGunRotationAndPosition();
		
		if (_hasShot)
			_weaponComponent.AttemptShoot(GetGlobalMousePosition().AngleToPoint(Position));

		if (_hasBombed)
			_weaponComponent.AttemptBomb();
		
		HandleRotation();
		
		// OLD MOVEMENT
		// if (!IsIdle)
		// 	Rotation += _rotationDirection * RotationSpeed * (float)delta;
		
		
		HandleAnimations();
	}
	
	public override void _PhysicsProcess(double delta)
	{
		// OLD MOVEMENT
		// Velocity = _movementDirection * Speed;
		
		Velocity = _movementDirection.Normalized() * _movementScalar * Speed;
		
		MoveAndSlide();
	}
	
	private void GetInput() 
	{
		// OLD MOVEMENT
		// _rotationDirection = Input.GetAxis("left", "right");
		// _movementDirection = Transform.X * Input.GetAxis("down", "up");

		// NEW MOVEMENT
		_movementDirection = new Vector2(
			Input.GetAxis("left", "right"), 
			Input.GetAxis("up", "down"));

		_hasShot = Input.IsActionPressed("shoot");
		_hasBombed = Input.IsActionJustPressed("bomb");
	}

	private void SetGunRotationAndPosition()
	{
		_gun.Rotation = (float)Mathf.LerpAngle(_gun.Rotation, GetGlobalMousePosition().AngleToPoint(Position) - Constants.HalfPiOffset, 0.5);
		_gun.Position = Position;
	}

	private void HandleAnimations()
	{
		_hull.Animation = IsIdle ? "default" : "moving";
		_hull.Play();
	}

	// NEW MOVEMENT
	private void HandleRotation()
	{
		if (_movementDirection == Vector2.Zero)
			return;


		Vector2 rotationVector = Vector2.FromAngle(Rotation).Normalized();
		
		if (rotationVector.DistanceTo(-_movementDirection.Normalized()) < 0.1f)
		{
			_movementScalar = 1;
			_rotationGoal -= 2 * Mathf.Pi;
		} else if (rotationVector.DistanceTo(_movementDirection.Normalized()) > 1.15f)
		{
			_movementScalar = 0;
			_rotationGoal = _movementDirection.Angle();
		}
		else
		{ 
			_movementScalar = 1;
			_rotationGoal = _movementDirection.Angle();
		}
		
		Rotation = Mathf.LerpAngle(rotationVector.Angle(), _rotationGoal, 0.05f);
	}
	
	private void OnHealthChange(float value)
	{
		if (value < 0)
			_animationPlayer.Play("effect_damage_blink", -1D, _statsComponent.ITime / 0.2f);
		else
			_animationPlayer.Play("effect_heal_blink", -1D);
	}
}
