using BitBuster.component;
using BitBuster.utils;
using Godot;

namespace BitBuster.entity.enemy;

public partial class TowerDetonator : IdleEnemy
{
	
	private Sprite2D _body;
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
		_body = GetNode<Sprite2D>("Body");
		_explodingComponent = GetNode<ExplodingComponent>("ExplodingComponent");
		_particleDeath = GetNode<GpuParticles2D>("ParticleDeath");
		
		_timeTillExplosion = 0f;
	}

	protected override void SetGunRotationAndPosition(float radian = 0)
	{
	}

	protected override void SetColor(Color color)
	{
		_body.SelfModulate = color;
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
			_body.Material.Set("shader_parameter/time", _timeTillExplosion);
			StatsComponent.Speed /= 4;
		}
		
		if ((_timeTillExplosion >= 1.5f || HealthComponent.CurrentHealth <= 0) && !_hasExploded)
		{
			_hasExploded = true;
			
			_body.Visible = false;
			StatsComponent.Speed = 0;
			HealthComponent.Damage(HealthComponent.CurrentHealth);

			_explodingComponent.Explode(StatsComponent.GetBombAttackData());
		}
		
		if (Position.DistanceTo(Player.Position) >= 64)
		{
			_timeTillExplosion = 0f;
			_body.Material.Set("shader_parameter/time", _timeTillExplosion);
			if (StatsComponent.Speed < 35)
				StatsComponent.Speed = 35;
		}
	}
	
	public override void HandleAnimations()
	{
	}
}
