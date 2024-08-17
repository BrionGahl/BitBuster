using BitBuster.world;
using Godot;

namespace BitBuster.entity.enemy.boss;

public partial class OilDemon : MovingEnemy
{
	private GlobalEvents _globalEvents;

	
	private GpuParticles2D _particleDeath;
	private Timer _mechanicsTimer;

	private float _mechanics;
	private int _mechanicsDir;
	private int _iteration;
	private bool _isRaging;
	
	public override void _Ready()
	{
		SetPhysicsProcess(false);
		base._Ready();
		
		_globalEvents = GetNode<GlobalEvents>("/root/GlobalEvents");

		_particleDeath = GetNode<GpuParticles2D>("ParticleDeath");
		_mechanicsTimer = GetNode<Timer>("MechanicsTimer");

		SpritesComponent.SetGunRotationAndPosition(CanSeePlayer(), Player.Position, Mathf.Pi/12);
		_mechanics = 0.0f;
		_iteration = 0;
		_isRaging = false;

		_mechanicsTimer.Timeout += OnMechanicsTimeout;
		NavigationServer2D.MapChanged += OnMapReady;
		AgentTimer.Timeout += OnAgentTimeout;
	}

	protected override void OnHealthIsZero()
	{
		SpritesComponent.Visible = false;
		Collider.SetDeferred("disabled", true);
		
		EntityStats.Speed = 0;
		HitboxComponent.SetDeferred("monitorable", false);
		HitboxComponent.SetDeferred("monitoring", false);
		
		HandleDrops();
		_particleDeath.Emitting = true;
		
		DeathAnimationTimer.Start();
		HasDied = true;
		
		_globalEvents.EmitBossKilledSignal();
	}

	protected override void OnDeathAnimationTimeout()
	{
		AnimationFinished = true;
	}

	public override void AttackAction(double delta)
	{
		AttemptToFree();
		if (HasDied)
			return;
		SpritesComponent.SetGunRotationAndPosition(CanSeePlayer(), Player.Position, Mathf.Pi / 4);

		if (_iteration <= 0)
		{
			_mechanics = RandomNumberGenerator.Randf();
			if (_mechanicsTimer.TimeLeft <= 0)
				_mechanicsTimer.Start();

			return;
		}
		
		if (_mechanics <= 0.3)
		{
			EntityStats.ProjectileBounces = 8;
			EntityStats.ProjectileSizeScalar = Vector2.One * 2;
			EntityStats.ProjectileSpeed = 200;
			EntityStats.ProjectileDamage *= 2;
			EntityStats.ProjectileCooldown = 1.33f;
			if (WeaponComponent.AttemptShoot(Player.Position.AngleToPoint(Position)))
				_iteration -= 10;
			EntityStats.ProjectileBounces = 0;
			EntityStats.ProjectileSizeScalar = Vector2.One;
			EntityStats.ProjectileSpeed = 100;
			EntityStats.ProjectileDamage /= 2;
			EntityStats.ProjectileCooldown = 0.08f;
		}
		
		// Bomb
		if (_mechanics > 0.3 && _mechanics <= 0.4)
		{
			if (WeaponComponent.AttemptBomb(Player.Position + new Vector2(RandomNumberGenerator.RandfRange(-10f, 10f), RandomNumberGenerator.RandfRange(-10f, 10f))))
				_iteration -= 5;
		}

		if (_mechanics > 0.4)
		{
			if (WeaponComponent.AttemptShoot(Player.Position.AngleToPoint(Position)))
				_iteration--;
			
		}
		// Always Spray
		
		if (EntityStats.CurrentHealth > EntityStats.MaxHealth / 5 && !_isRaging) 
			return;
		Modulate = Colors.DarkRed;
		_isRaging = true;
		EntityStats.ProjectileCount = 40;
		_mechanicsTimer.WaitTime = 2.5f;
		EntityStats.Speed = 75f;
		EntityStats.ProjectileDamage = 2;
	}
	

	protected override void OnAgentTimeout()
	{
		Agent.TargetPosition = Target == Vector2.Zero ? Player.Position : Target;
	}

	private void OnMapReady(Rid rid)
	{
		SetPhysicsProcess(true);
		AgentTimer.Start();
	}
	
	private void OnMechanicsTimeout()
	{
		_iteration = WeaponComponent.BulletCount;
	}
}
