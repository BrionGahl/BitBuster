using BitBuster.component;
using Godot;

namespace BitBuster.entity.enemy.idle;

public partial class TowerBurstShield : IdleEnemy
{
	private CollisionShape2D _collider;
	private GpuParticles2D _particleDeath;
	private OverhealBurstComponent _overhealBurstComponent;
	
	private bool _hasDied;
	private bool _animationFinished;
	private bool _hasBurst;
	private float _timeTillBurst;

	public override void _Ready()
	{
		base._Ready();
		_collider = GetNode<CollisionShape2D>("Collider");
		_overhealBurstComponent = GetNode<OverhealBurstComponent>("OverhealBurstComponent");
		_particleDeath = GetNode<GpuParticles2D>("ParticleDeath");

		_timeTillBurst = 0f;
		SpritesComponent.SetBodyMaterialProperty("shader_parameter/time", _timeTillBurst);

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
		QueueFree();
	}

	public override void AttackAction(double delta)
	{
		if (_timeTillBurst >= 6.0f)
		{
			_timeTillBurst = 0f;
			_overhealBurstComponent.Burst(75f);
		}

		_timeTillBurst += (float)delta;
		SpritesComponent.SetBodyMaterialProperty("shader_parameter/time", _timeTillBurst);
	}
}
