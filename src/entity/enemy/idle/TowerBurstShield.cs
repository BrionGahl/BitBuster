using BitBuster.component;
using BitBuster.world;
using Godot;

namespace BitBuster.entity.enemy.idle;

public partial class TowerBurstShield : IdleEnemy
{
	private OverhealBurstComponent _overhealBurstComponent;
	
	private bool _hasBurst;
	private float _timeTillBurst;

	public override void _Ready()
	{
		base._Ready();
		_overhealBurstComponent = GetNode<OverhealBurstComponent>("OverhealBurstComponent");

		_timeTillBurst = 0f;
		SpritesComponent.SetBodyMaterialProperty("shader_parameter/time", _timeTillBurst);

	}
	
	
	protected override void OnHealthIsZero()
	{
		SpritesComponent.Visible = false;
		Collider.SetDeferred("disabled", true);
		
		_overhealBurstComponent.Visible = false;
		_overhealBurstComponent.SetCollisionLayerValue((int)BBCollisionLayer.Projectile, false);
		
		HitboxComponent.SetDeferred("monitorable", false);
		HitboxComponent.SetDeferred("monitoring", false);
	
		CleanAndRebake();
		HandleDrops();
		
		ParticleDeath.Emitting = true;
		
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

		if (_timeTillBurst >= 6.0f)
		{
			_timeTillBurst = 0f;
			_overhealBurstComponent.Burst(75f);
		}

		_timeTillBurst += (float)delta;
		SpritesComponent.SetBodyMaterialProperty("shader_parameter/time", _timeTillBurst);
	}
}
