using BitBuster.utils;
using Godot;

namespace BitBuster.entity.enemy.idle;

public partial class DefaultTower : IdleEnemy
{
	private CollisionShape2D _collider;
	private GpuParticles2D _particleDeath;

	public override void _Ready()
	{
		base._Ready();
		_collider = GetNode<CollisionShape2D>("Collider");
		_particleDeath = GetNode<GpuParticles2D>("ParticleDeath");
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

		SpritesComponent.SetGunRotationAndPosition(CanSeePlayer(), Player.Position, Mathf.Pi/12);
		if (CanSeePlayer() && RandomNumberGenerator.Randf() > 0.3f)
			WeaponComponent.AttemptShoot(Player.Position.AngleToPoint(Position));
	}
}
