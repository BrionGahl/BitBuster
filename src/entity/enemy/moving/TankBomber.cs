using BitBuster.component;
using BitBuster.utils;
using BitBuster.world;
using Godot;

namespace BitBuster.entity.enemy.moving;

public partial class TankBomber : MovingEnemy
{
	private GlobalEvents _globalEvents;

	private ExplodingComponent _explodingComponent;
	
	private float _timeTillExplosion;
	private bool _hasExploded;
	
	public override void _Ready()
	{
		base._Ready();
		_globalEvents = GetNode<GlobalEvents>("/root/GlobalEvents");
		
		_explodingComponent = GetNode<ExplodingComponent>("ExplodingComponent");
		
		_timeTillExplosion = 0f;
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

		if (Position.DistanceTo(Player.Position) < 48 && RandomNumberGenerator.Randf() > 0.3f)
		{
			WeaponComponent.AttemptBomb(Position, -Rotation);
		}
	}
}
