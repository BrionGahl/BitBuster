using BitBuster.utils;
using BitBuster.world;
using Godot;

namespace BitBuster.entity.enemy.boss;

public partial class Firespitter : IdleEnemy
{
	private GpuParticles2D _particleDeath;
	private Timer _mechanicsTimer;
	
	private float _mechanics;
	private int _iteration;

	public override void _Ready()
	{
		base._Ready();
		_particleDeath = GetNode<GpuParticles2D>("ParticleDeath");
		_mechanicsTimer = GetNode<Timer>("MechanicsTimer");
		
		SpritesComponent.SetGunRotationAndPosition(CanSeePlayer(), Player.Position, Mathf.Pi/12);

		_mechanics = 0.0f;
		_iteration = 0;

		_mechanicsTimer.Timeout += OnMechanicsTimeout;
	}
	
	protected override void OnHealthIsZero()
	{
		SpritesComponent.Visible = false;
		Collider.SetDeferred("disabled", true);
		HitboxComponent.SetDeferred("monitorable", false);
		HitboxComponent.SetDeferred("monitoring", false);
	
		CleanAndRebake();

		_particleDeath.Emitting = true;
		
		DeathAnimationTimer.Start();
		HasDied = true;
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

		// Logger.Log.Information("{@A}", _iteration);
		
		if (_iteration <= 0)
		{
			_mechanics = RandomNumberGenerator.Randf();
			ChangeMechanic();
			if (_mechanicsTimer.TimeLeft <= 0)
				_mechanicsTimer.Start();

			return;
		}
		
		if (_mechanics <= 0.3)
		{
			int r = RandomNumberGenerator.RandiRange(1, 4);
			SpritesComponent.SetGunRotationAndPosition(false, Player.Position, r * Mathf.Pi / 2);
			if (WeaponComponent.AttemptShoot(r * Mathf.Pi / 2))
				_iteration -= 8;
		}
		
		// Spray
		if (_mechanics > 0.3 && _mechanics <= 0.7)
		{
			SpritesComponent.SetGunRotationAndPosition(false, Player.Position, _iteration * Mathf.Pi / WeaponComponent.BulletCount);
			if (WeaponComponent.AttemptShoot(_iteration * Mathf.Pi / WeaponComponent.BulletCount))
				_iteration--;
		}

		// Three Shots
		if (_mechanics > 0.7)
		{
			SpritesComponent.SetGunRotationAndPosition(CanSeePlayer(), Player.Position, Mathf.Pi / 4);
			if (WeaponComponent.AttemptShoot(Player.Position.AngleToPoint(Position)))
				_iteration -= 5;
		}
		

	}
	
	private void ChangeMechanic()
	{
		if (_mechanics <= 0.3)
		{
			EntityStats.ProjectileBounces = 4;
			EntityStats.ProjectileDamage = 2;
			EntityStats.ProjectileCooldown = 3f;
			EntityStats.ProjectileSpeed = 50f;
			EntityStats.ProjectileSizeScalar = new Vector2(120, 1);
			EntityStats.ProjectileWeaponType = 0;
			EntityStats.ProjectileBulletType = BulletType.Piercing;
		} else if (_mechanics > 0.3 && _mechanics <= 0.7)
		{
			EntityStats.ProjectileBounces = 0;
			EntityStats.ProjectileDamage = 3;
			EntityStats.ProjectileCooldown = 0.2f;
			EntityStats.ProjectileSpeed = 100f;
			EntityStats.ProjectileSizeScalar = new Vector2(1, 1);
			EntityStats.ProjectileWeaponType = WeaponType.Quad;
			EntityStats.ProjectileBulletType = BulletType.Invulnerable;
		} else if (_mechanics > 0.7)
		{
			EntityStats.ProjectileBounces = 2;
			EntityStats.ProjectileDamage = 3;
			EntityStats.ProjectileCooldown = 1.5f;
			EntityStats.ProjectileSpeed = 100f;
			EntityStats.ProjectileSizeScalar = new Vector2(4, 4);
			EntityStats.ProjectileWeaponType = 0;
			EntityStats.ProjectileBulletType = 0;
			EntityStats.ProjectileBulletType = BulletType.Invulnerable;

		}
	}
	
	private void OnMechanicsTimeout()
	{
		_iteration = WeaponComponent.BulletCount;
	}
}
