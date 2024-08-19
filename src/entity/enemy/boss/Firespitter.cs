using BitBuster.resource;
using BitBuster.utils;
using BitBuster.weapon;
using BitBuster.world;
using Godot;

namespace BitBuster.entity.enemy.boss;

public partial class Firespitter : IdleEnemy
{
	private GlobalEvents _globalEvents;
	
	private Timer _mechanicsTimer;
	
	private float _mechanics;
	private int _mechanicsDir;
	private int _iteration;

	public override void _Ready()
	{
		base._Ready();

		_globalEvents = GetNode<GlobalEvents>("/root/GlobalEvents");
		
		_mechanicsTimer = GetNode<Timer>("MechanicsTimer");
		
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
		HandleDrops();
		
		HasDied = true;
		_globalEvents.EmitBossKilledSignal();
		
		ParticleDeath.Emitting = true;
	}

	protected override void OnParticleDeathFinished()
	{
		AnimationFinished = true;
	}

	public override void AttackAction(double delta)
	{
		AttemptToFree();
		if (HasDied)
			return;
		
		if (_iteration <= 0)
		{
			_mechanics = RandomNumberGenerator.Randf();
			ChangeMechanic();
			if (_mechanicsTimer.TimeLeft <= 0)
				_mechanicsTimer.Start();

			return;
		}
		
		// Beam
		if (_mechanics <= 0.3)
		{
			SpritesComponent.SetGunRotation(false, Player.Position, _mechanicsDir * Mathf.Pi / 2);
			if (WeaponComponent.AttemptShoot(Position, _mechanicsDir * Mathf.Pi / 2))
				_iteration -= 5;
			_mechanicsDir = RandomNumberGenerator.RandiRange(1, 4);
		}
		
		// Spray
		if (_mechanics > 0.3 && _mechanics <= 0.7)
		{
			int dir = _mechanicsDir == 1 ? 1 : -1;
			SpritesComponent.SetGunRotation(false, Player.Position, dir * _iteration * Mathf.Pi / WeaponComponent.BulletCount);
			if (WeaponComponent.AttemptShoot(Position, dir * _iteration * Mathf.Pi / WeaponComponent.BulletCount))
				_iteration--;
		}

		// Three Shots
		if (_mechanics > 0.7)
		{
			SpritesComponent.SetGunRotation(CanSeePlayer(), Player.Position, Mathf.Pi / 4);
			if (WeaponComponent.AttemptShoot(Position, Player.Position.AngleToPoint(Position)))
				_iteration -= 5;
		}
	}
	
	private void ChangeMechanic()
	{
		if (_mechanics <= 0.3)
		{
			_mechanicsDir = RandomNumberGenerator.RandiRange(1, 4);
			EntityStats.ProjectileBounces = 3;
			EntityStats.ProjectileDamage = 1;
			EntityStats.ProjectileCooldown = 3f;
			EntityStats.ProjectileSpeed = 50f;
			EntityStats.ProjectileSizeScalar = new Vector2(130, 1);
			EntityStats.ProjectileWeaponType = 0;
			EntityStats.ProjectileBulletType = BulletType.Piercing | BulletType.Invulnerable;
		} else if (_mechanics > 0.3 && _mechanics <= 0.7)
		{
			_mechanicsDir = RandomNumberGenerator.RandiRange(0, 1);
			EntityStats.ProjectileBounces = 0;
			EntityStats.ProjectileDamage = 1;
			EntityStats.ProjectileCooldown = 0.2f;
			EntityStats.ProjectileSpeed = 100f;
			EntityStats.ProjectileSizeScalar = new Vector2(1, 1);
			EntityStats.ProjectileWeaponType = WeaponType.Quad;
			EntityStats.ProjectileBulletType = BulletType.Invulnerable;
		} else if (_mechanics > 0.7)
		{
			EntityStats.ProjectileBounces = 2;
			EntityStats.ProjectileDamage = 1;
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
