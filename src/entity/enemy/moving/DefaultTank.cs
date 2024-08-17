using BitBuster.utils;
using Godot;

namespace BitBuster.entity.enemy.moving;

public partial class DefaultTank : MovingEnemy
{
	private GpuParticles2D _particleDeath;
	
	public override void _Ready()
	{
		SetPhysicsProcess(false);

		base._Ready();
		_particleDeath = GetNode<GpuParticles2D>("ParticleDeath");
		
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

		SpritesComponent.SetGunRotationAndPosition(CanSeePlayer(), Player.Position, Mathf.Pi/12);
		if (CanSeePlayer() && RandomNumberGenerator.Randf() > 0.3f)
			WeaponComponent.AttemptShoot(Player.Position.AngleToPoint(Position));
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
}
