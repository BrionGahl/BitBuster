using BitBuster.component;
using BitBuster.utils;
using Godot;

namespace BitBuster.entity.enemy.idle;

public partial class TowerDetonator : IdleEnemy
{
	private ExplodingComponent _explodingComponent;
	
	private bool _hasExploded;
	private float _timeTillExplosion;

	public override void _Ready()
	{
		base._Ready();
		
		_explodingComponent = GetNode<ExplodingComponent>("ExplodingComponent");
		_explodingComponent.EntityStats = EntityStats;
		
		_timeTillExplosion = 0f;
		SpritesComponent.SetBodyMaterialProperty("shader_parameter/time", _timeTillExplosion);
		
		_explodingComponent.ExplodingEmitter.Finished += OnExplodeFinished;
	}
	
	
	protected override void OnHealthIsZero()
	{
		Collider.SetDeferred("disabled", true);
		HitboxComponent.SetDeferred("monitorable", false);
		HitboxComponent.SetDeferred("monitoring", false);
	
		CleanAndRebake();
		HandleDrops();

		HasDied = true;
	}

	protected override void OnParticleDeathFinished()
	{
		QueueFree();
	}

	private void OnExplodeFinished()
	{
		QueueFree();
	}

	public override void AttackAction(double delta)
	{
		if (Position.DistanceTo(Player.Position) < 64)
		{
			_timeTillExplosion += (float)delta;
			_explodingComponent.RadiusIndicatorEmitter.Emitting = true;
			SpritesComponent.SetBodyMaterialProperty("shader_parameter/time", _timeTillExplosion);
			EntityStats.Speed /= 4;
		}
		
		if ((_timeTillExplosion >= 1.5f || HealthComponent.CurrentHealth <= 0) && !_hasExploded)
		{
			_hasExploded = true;
			
			SpritesComponent.Visible = false;
			HealthComponent.Damage(HealthComponent.CurrentHealth);

			_explodingComponent.Explode(EntityStats.GetBombAttackData());
		}
		
		if (Position.DistanceTo(Player.Position) >= 64)
		{
			_timeTillExplosion = 0f;
			_explodingComponent.RadiusIndicatorEmitter.Emitting = false;
			SpritesComponent.SetBodyMaterialProperty("shader_parameter/time", _timeTillExplosion);
		}
	}
}
