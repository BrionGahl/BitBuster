using BitBuster.utils;
using Godot;

namespace BitBuster.entity.enemy.boss;

public partial class Firespitter : IdleEnemy
{
	private GpuParticles2D _particleDeath;

	private float _mechanics;
	private int _iteration;

	public override void _Ready()
	{
		base._Ready();
		_particleDeath = GetNode<GpuParticles2D>("ParticleDeath");
		SpritesComponent.SetGunRotationAndPosition(CanSeePlayer(), Player.Position, Mathf.Pi/12);

		_mechanics = 0.0f;
		_iteration = 0;
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

		if (_iteration < 0)
		{
			_mechanics = RandomNumberGenerator.Randf();
			_iteration = WeaponComponent.BulletCount;
		}

		// Spray
		if (CanSeePlayer() && _iteration >= 0)
		{
			SpritesComponent.SetGunRotationAndPosition(false, Player.Position, _iteration * Mathf.Pi / WeaponComponent.BulletCount);
			WeaponComponent.AttemptShoot(_iteration * Mathf.Pi / WeaponComponent.BulletCount);
			_iteration--;
		}
			
	}
}
