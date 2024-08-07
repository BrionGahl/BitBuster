using BitBuster.component;
using BitBuster.world;
using Godot;

namespace BitBuster.entity.enemy.moving;

public partial class Detonator : MovingEnemy
{
	private AnimatedSprite2D _hull;
	private CollisionShape2D _collider;
	private ExplodingComponent _explodingComponent;
	
	private float _timeTillExplosion;
	private bool _hasExploded;
	
	private int _movementScalar;
	private float _rotationGoal;
	
	public override void _Ready()
	{
		SetPhysicsProcess(false);
		base._Ready();
		_collider = GetNode<CollisionShape2D>("Collider");
		_explodingComponent = GetNode<ExplodingComponent>("ExplodingComponent");
		_explodingComponent.StatsComponent = StatsComponent;

		_timeTillExplosion = 0f;
		SpritesComponent.SetBodyMaterialProperty("shader_parameter/time", _timeTillExplosion);
		
		NavigationServer2D.MapChanged += OnMapReady;
		AgentTimer.Timeout += OnAgentTimeout;
	}

	protected override void OnHealthIsZero()
	{
		_collider.SetDeferred("disabled", true);
		
		HitboxComponent.SetDeferred("monitorable", false);
		HitboxComponent.SetDeferred("monitoring", false);
		
		DeathAnimationTimer.Start();
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
			StatsComponent.Speed = 0;
			HealthComponent.Damage(HealthComponent.CurrentHealth);

			_explodingComponent.Explode(StatsComponent.GetBombAttackData());
		}
		
		if (Position.DistanceTo(Player.Position) >= 64)
		{
			_timeTillExplosion = 0f;
			SpritesComponent.SetBodyMaterialProperty("shader_parameter/time", _timeTillExplosion);
			if (StatsComponent.Speed < 35)
				StatsComponent.Speed = 35;
		}
	}
	
	public override void MoveAction(double delta)
	{
		Vector2 goalVector = (Agent.GetNextPathPosition() - GlobalPosition).Normalized();
		
		if (goalVector == Vector2.Zero)
			return;

		Vector2 rotationVector = Vector2.FromAngle(Rotation).Normalized();
		if (rotationVector.DistanceTo(-goalVector.Normalized()) < 0.1f)
		{
			_movementScalar = 1;
			_rotationGoal -= 2 * Mathf.Pi;
		} else if (rotationVector.DistanceTo(goalVector.Normalized()) > 0.6f)
		{
			_movementScalar = 0;
			_rotationGoal = goalVector.Angle();
		}
		else
		{ 
			_movementScalar = 1;
			_rotationGoal = goalVector.Angle();
		}
		
		Rotation = Mathf.LerpAngle(rotationVector.Angle(), _rotationGoal, 0.05f);
		
		Velocity = goalVector.Normalized() * _movementScalar * Speed;
		MoveAndSlide();
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
