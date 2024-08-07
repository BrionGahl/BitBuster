using BitBuster.component;
using Godot;

namespace BitBuster.entity.enemy.idle;

public partial class TowerDetonator : IdleEnemy
{
	private CollisionShape2D _collider;
	private GpuParticles2D _particleDeath;
	private ExplodingComponent _explodingComponent;
	
	private bool _hasDied;
	private bool _animationFinished;
	private bool _hasExploded;
	private float _timeTillExplosion;

	public override void _Ready()
	{
		base._Ready();
		_collider = GetNode<CollisionShape2D>("Collider");
		_explodingComponent = GetNode<ExplodingComponent>("ExplodingComponent");
		_particleDeath = GetNode<GpuParticles2D>("ParticleDeath");
		
		_timeTillExplosion = 0f;
		SpritesComponent.SetBodyMaterialProperty("shader_parameter/time", _timeTillExplosion);
	}
	
	
	protected override void OnHealthIsZero()
	{
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
		if (Position.DistanceTo(Player.Position) < 64)
		{
			_timeTillExplosion += (float)delta;
			SpritesComponent.SetBodyMaterialProperty("shader_parameter/time", _timeTillExplosion);
			StatsComponent.Speed /= 4;
		}
		
		if ((_timeTillExplosion >= 1.5f || HealthComponent.CurrentHealth <= 0) && !_hasExploded)
		{
			_hasExploded = true;
			
			SpritesComponent.Visible = false;
			HealthComponent.Damage(HealthComponent.CurrentHealth);

			_explodingComponent.Explode(StatsComponent.GetBombAttackData());
		}
		
		if (Position.DistanceTo(Player.Position) >= 64)
		{
			_timeTillExplosion = 0f;
			SpritesComponent.SetBodyMaterialProperty("shader_parameter/time", _timeTillExplosion);
		}
	}
}
