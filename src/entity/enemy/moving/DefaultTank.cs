using BitBuster.utils;
using Godot;

namespace BitBuster.entity.enemy.moving;

public partial class DefaultTank : MovingEnemy
{
	public override void _Ready()
	{
		SetPhysicsProcess(false);

		base._Ready();
		
		NavigationServer2D.MapChanged += OnMapReady;
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

		SpritesComponent.SetGunRotation(CanSeePlayer(), Player.Position, Mathf.Pi/12);
		if (CanSeePlayer() && RandomNumberGenerator.Randf() > 0.3f)
			WeaponComponent.AttemptShoot(Position, Player.Position.AngleToPoint(Position));
	}

	private void OnMapReady(Rid rid)
	{
		SetPhysicsProcess(true);
		AgentTimer.Start();
	}
}
