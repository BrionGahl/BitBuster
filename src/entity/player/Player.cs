using BitBuster.component;
using BitBuster.item;
using BitBuster.utils;
using BitBuster.world;
using Godot;

namespace BitBuster.entity.player;

public partial class Player : Entity
{
	[Signal]
	public delegate void DiedEventHandler();

	
	private GlobalEvents _globalEvents;
	private Global _global;	

	[Export] 
	private WeaponComponent _weaponComponent;

	[Export] 
	private HealthComponent _healthComponent;
	
	[Export]
	private AimIndicatorComponent _aimIndicatorComponent;

	private ItemPickupHitbox _itemPickupHitbox;
	
	private float Speed
	{
		get => EntityStats.Speed;
		set => EntityStats.Speed = value;
	}
	private float RotationSpeed => Speed / 25;
	private bool IsIdle => Velocity.Equals(Vector2.Zero);
	
	private AnimatedSprite2D _gun;
	private AnimatedSprite2D _hull;
	private AnimationPlayer _animationPlayer;
	private Timer _doorEnterTimer;

	private Vector2 _movementDirection;
	private float _rotationGoal;
	private int _movementScalar;
	
	private bool _hasShot;
	private bool _hasBombed;

	private bool _deactivated;

	public WeaponComponent WeaponComponent
	{
		get => _weaponComponent;
		private set => _weaponComponent = value;
	}
	
	public HealthComponent HealthComponent
	{
		get => _healthComponent;
		private set => _healthComponent = value;
	}
	
	public bool CanEnterDoor => _doorEnterTimer.TimeLeft <= 0;
	
	public override void _Ready()
	{
		Logger.Log.Information("Loading player...");
		base._Ready();

		_global = GetNode<Global>("/root/Global");
		_globalEvents = GetNode<GlobalEvents>("/root/GlobalEvents");
		
		_gun = GetNode<AnimatedSprite2D>("Gun");
		_hull = GetNode<AnimatedSprite2D>("Hull");
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_doorEnterTimer = GetNode<Timer>("DoorEnterTimer");

		_itemPickupHitbox = GetNode<ItemPickupHitbox>("ItemPickupHitbox");
		_itemPickupHitbox.EntityStats = EntityStats;
		
		_weaponComponent.EntityStats = EntityStats;
		_healthComponent.EntityStats = EntityStats;
		
		_globalEvents.IncrementAndGenerateLevel += OnIncrementAndGenerateLevel;
		_healthComponent.HealthChange += OnHealthChange;
		_healthComponent.HealthIsZero += OnHealthIsZero;
	}
	
	public override void _Process(double delta)
	{
		if (_deactivated)
			return;
		
		GetInput();
		_gun.Rotation = (float)Mathf.RotateToward(_gun.Rotation, GetGlobalMousePosition().AngleToPoint(Position) - Constants.HalfPiOffset, 0.5);
		
		_aimIndicatorComponent.ClearPoints();
		_aimIndicatorComponent.DrawLine(GetGlobalMousePosition());
		
		if (_hasShot)
			_weaponComponent.AttemptShoot(Position, GetGlobalMousePosition().AngleToPoint(Position));

		if (_hasBombed)
			_weaponComponent.AttemptBomb(Position, GetGlobalMousePosition().AngleToPoint(Position));
		
		HandleRotation();
		HandleAnimations();
		
	}
	
	public override void _PhysicsProcess(double delta)
	{
		Velocity = _movementDirection.Normalized() * _movementScalar * Speed;
		
		MoveAndSlide();
	}

	public void EnterDoor()
	{
		_doorEnterTimer.Start();
	}
	
	private void GetInput()
	{
		_movementDirection = new Vector2(
			Input.GetAxis("left", "right"), 
			Input.GetAxis("up", "down"));

		_hasShot = Input.IsActionPressed("shoot");
		_hasBombed = Input.IsActionJustPressed("bomb");
	}

	private void HandleAnimations()
	{
		_gun.Animation = !_hasShot ? "default" : "shot"; 
		_gun.Play();
		_hull.Animation = IsIdle ? "default" : "moving";
		_hull.Play();
	}

	private void HandleRotation()
	{
		if (_movementDirection == Vector2.Zero)
			return;


		Vector2 rotationVector = Vector2.FromAngle(Rotation).Normalized();
		
		if (rotationVector.DistanceTo(-_movementDirection.Normalized()) < 0.1f)
		{
			_movementScalar = 1;
			_rotationGoal -= 2 * Mathf.Pi;
		} else if (rotationVector.DistanceTo(_movementDirection.Normalized()) > 1.25f)
		{
			_movementScalar = 0;
			_rotationGoal = _movementDirection.Angle();
		}
		else
		{ 
			_movementScalar = 1;
			_rotationGoal = _movementDirection.Angle();
		}
		
		Rotation = Mathf.RotateToward(rotationVector.Angle(), _rotationGoal, 0.05f);
	}
	
	private void OnIncrementAndGenerateLevel()
	{
		EntityStats.Overheal += EntityStats.OverhealRegen;
	}
	
	private void OnHealthChange(float value)
	{
		if (value < 0)
			_animationPlayer.Play("effect_damage_blink", -1D, EntityStats.ITime / 0.2f);
		else
			_animationPlayer.Play("effect_heal_blink");
	}

	protected override void OnParticleDeathFinished()
	{
		EmitSignal(SignalName.Died);
	}
	
	private void OnHealthIsZero()
	{
		_hull.Visible = false;
		_gun.Visible = false;
		_aimIndicatorComponent.Visible = false;
		
		Speed = 0;
		_deactivated = true;
		
		if (!ParticleDeath.Emitting)
			ParticleDeath.Emitting = true;
	}
	
	
	public override void _ExitTree()
	{
		_globalEvents.IncrementAndGenerateLevel -= OnIncrementAndGenerateLevel;
	}
}
