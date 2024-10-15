using BitBuster.weapon;
using BitBuster.world;
using Godot;

namespace BitBuster.entity.enemy.boss;

public partial class OilDemon : MovingEnemy
{
	private GlobalEvents _globalEvents;
	
	private Timer _mechanicsTimer;

	private float _mechanics;
	private int _mechanicsDir;
	private int _iteration;
	private bool _isRaging;
	
	public override void _Ready()
	{
		base._Ready();
		
		_globalEvents = GetNode<GlobalEvents>("/root/GlobalEvents");

		_mechanicsTimer = GetNode<Timer>("MechanicsTimer");

		_mechanics = 0.0f;
		_iteration = 0;
		_isRaging = false;

		_mechanicsTimer.Timeout += OnMechanicsTimeout;
	}

	protected override void OnHealthIsZero()
	{
		SpritesComponent.Visible = false;
		Collider.SetDeferred("disabled", true);
		
		EntityStats.Speed = 0;
		HitboxComponent.SetDeferred("monitorable", false);
		HitboxComponent.SetDeferred("monitoring", false);
		
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
		SpritesComponent.SetGunRotation(CanSeePlayer(), Player.Position, Mathf.Pi / 4);

		if (_iteration <= 0)
		{
			_mechanics = RandomNumberGenerator.Randf();
			if (_mechanicsTimer.TimeLeft <= 0)
				_mechanicsTimer.Start();

			return;
		}
		
		if (_mechanics <= 0.3)
		{
			EntityStats.ProjectileBounces = 4;
			EntityStats.ProjectileSizeScalar = Vector2.One * 2;
			EntityStats.ProjectileSpeed = 150;
			EntityStats.ProjectileDamage *= 2;
			EntityStats.ProjectileCooldown = 1.66f;
			if (WeaponComponent.AttemptShoot(Position, Player.Position.AngleToPoint(Position)))
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
			if (WeaponComponent.AttemptBomb(Player.Position + new Vector2(RandomNumberGenerator.RandfRange(-10f, 10f), RandomNumberGenerator.RandfRange(-10f, 10f)), 0f))
				_iteration -= 5;
		}

		if (_mechanics > 0.4)
		{
			if (WeaponComponent.AttemptShoot(Position, Player.Position.AngleToPoint(Position)))
				_iteration--;
			
		}
		// Always Spray
		
		if (EntityStats.CurrentHealth > EntityStats.MaxHealth / 5 && !_isRaging) 
			return;
		
		SpritesComponent.Gun.Modulate = Colors.DarkRed;
		SpritesComponent.Body.Modulate = Colors.DarkRed;
		
		_isRaging = true;
		_mechanicsTimer.WaitTime = 2.5f;
		EntityStats.Speed = 75f;
		HitboxComponent.ApplyStatus(EffectType.Fire);
	}
	
	private void OnMechanicsTimeout()
	{
		_iteration = WeaponComponent.BulletCount;
	}
}
