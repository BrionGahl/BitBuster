using System;
using BitBuster.component;
using BitBuster.data;
using BitBuster.utils;
using Godot;

namespace BitBuster.entity.enemy;

public partial class Detonator : MovingEnemy
{
	
	private AnimatedSprite2D _hull;
	private GpuParticles2D _explodeEmitter;
	private Area2D _hitbox;

	private float _timer;
	
	public override void _Ready()
	{
		SetPhysicsProcess(false);

		base._Ready();
		_hull = GetNode<AnimatedSprite2D>("Hull");
		_hitbox = GetNode<Area2D>("Hitbox");
		_explodeEmitter = GetNode<GpuParticles2D>("ExplodeEmitter");

		_timer = 1.5f;
		
		NavigationServer2D.MapChanged += OnMapReady;
		AgentTimer.Timeout += OnAgentTimeout;
	}

	public override void SetGunRotationAndPosition(float radian = 0)
	{
	}

	public override void HandleAnimations()
	{
		_hull.Animation = IsIdle ? "default" : "moving";
		_hull.Play();
	}

	public override void AttackAction(double delta)
	{
		if (Position.DistanceTo(Player.Position) > 64)
		{
			_timer = 1.5f;
			if (StatsComponent.Speed < 35)
				StatsComponent.Speed *= 4;
			return;
		}

		_timer -= (float)delta;
		StatsComponent.Speed /= 4;
		
		if (_timer <= 0)
		{
			Logger.Log.Information("Detonator boom...");
			_explodeEmitter.Emitting = true;
			
			if (!_hitbox.Monitoring)
				return;
			
			foreach (var area in _hitbox.GetOverlappingAreas())
			{
				if (area is HitboxComponent)
				{
					Logger.Log.Information("Hitbox hit at " + area.Name);

					HitboxComponent hitboxComponent = area as HitboxComponent;
					hitboxComponent.Damage(new AttackData(2f, 0, 0, EffectType.Normal, SourceType.Enemy));
				}
			}

			_hitbox.Monitoring = false;
		}
	}
	
	public override void MoveAction(double delta)
	{
		Vector2 goalVector = (Agent.GetNextPathPosition() - GlobalPosition).Normalized();
		if (!IsIdle)
			Rotation = Mathf.LerpAngle(Rotation, (goalVector.Angle() + Constants.HalfPiOffset), RotationSpeed / 60 );
		Velocity = new Vector2((float)(-Speed * Math.Sin(-Rotation)), (float)(-Speed * Math.Cos(-Rotation)));
		
		MoveAndSlide();
	}

	public override void OnAgentTimeout()
	{
		Agent.TargetPosition = Target == Vector2.Zero ? Player.Position : Target;
	}

	private void OnMapReady(Rid rid)
	{
		SetPhysicsProcess(true);
	}
}
