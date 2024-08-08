using BitBuster.utils;
using Godot;

namespace BitBuster.entity.enemy.boss;

public partial class Firespitter : IdleEnemy
{
	private CollisionShape2D _collider;
	private GpuParticles2D _particleDeath;

	private float _mechanics;
	private int _iteration;

	public override void _Ready()
	{
		base._Ready();
		_collider = GetNode<CollisionShape2D>("Collider");
		_particleDeath = GetNode<GpuParticles2D>("ParticleDeath");
		SpritesComponent.SetGunRotationAndPosition(CanSeePlayer(), Player.Position, Mathf.Pi/12);

		_mechanics = 0.0f;
		_iteration = 0;
	}
	
	protected override void OnHealthIsZero()
	{
		SpritesComponent.Visible = false;
		_collider.SetDeferred("disabled", true);
		HitboxComponent.SetDeferred("monitorable", false);
		HitboxComponent.SetDeferred("monitoring", false);
	
		CleanAndRebake();

		_particleDeath.Emitting = true;
		
		DeathAnimationTimer.Start();
		_hasDied = true;
	}

	protected override void OnDeathAnimationTimeout()
	{
		_animationFinished = true;
	}

	public override void AttackAction(double delta)
	{
		AttemptToFree();
		if (_hasDied)
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
