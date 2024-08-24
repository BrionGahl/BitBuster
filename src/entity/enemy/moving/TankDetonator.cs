using BitBuster.component;
using BitBuster.utils;
using Godot;

namespace BitBuster.entity.enemy.moving;

public partial class TankDetonator : MovingEnemy
{
	private ExplodingComponent _explodingComponent;
	
	private float _timeTillExplosion;
	private bool _hasExploded;
	
	public override void _Ready()
	{
		SetPhysicsProcess(false);
		base._Ready();
		
		_explodingComponent = GetNode<ExplodingComponent>("ExplodingComponent");
		_explodingComponent.EntityStats = EntityStats;

		_timeTillExplosion = 0f;
		SpritesComponent.SetBodyMaterialProperty("shader_parameter/time", _timeTillExplosion);
		
		NavigationServer2D.MapChanged += OnMapReady;
		_explodingComponent.ExplodingEmitter.Finished += OnExplodeFinished;
	}

	protected override void OnHealthIsZero()
	{
		Collider.SetDeferred("disabled", true);
		
		HitboxComponent.SetDeferred("monitorable", false);
		HitboxComponent.SetDeferred("monitoring", false);
		
		HandleDrops();
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
			SpritesComponent.SetBodyMaterialProperty("shader_parameter/time", _timeTillExplosion);
			EntityStats.Speed = 0;
		}
		
		if ((_timeTillExplosion >= 1.5f || HealthComponent.CurrentHealth <= 0) && !_hasExploded)
		{
			_hasExploded = true;
			
			SpritesComponent.Visible = false;
			EntityStats.Speed = 0;
			HealthComponent.Damage(HealthComponent.CurrentHealth);

			_explodingComponent.Explode(EntityStats.GetBombAttackData());
		}
		
		if (Position.DistanceTo(Player.Position) >= 64)
		{
			_timeTillExplosion = 0f;
			SpritesComponent.SetBodyMaterialProperty("shader_parameter/time", _timeTillExplosion);
			if (EntityStats.Speed < 35)
				EntityStats.Speed = 35;
		}
	}

	private void OnMapReady(Rid rid)
	{
		SetPhysicsProcess(true);
		AgentTimer.Start();
	}
}
