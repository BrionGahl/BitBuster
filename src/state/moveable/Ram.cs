using Godot;
using System;
using BitBuster.component;
using BitBuster.data;
using BitBuster.entity.enemy;
using BitBuster.state;
using BitBuster.utils;

public partial class Ram : State
{
	
	private Enemy _parent;
	private NavigationAgent2D _agent;
	private Timer _agentTimer;
	private VisibleOnScreenNotifier2D _notifier;
	private StateMachine _stateMachine;
	private GpuParticles2D _explodeEmitter;
	
	private Area2D _hitbox;
	private float _timer;
	
	public override void Init()
	{
		_parent = GetParent().GetParent<CharacterBody2D>() as Enemy;
		_agent = GetParent().GetParent().GetNode<NavigationAgent2D>("Agent");
		_agentTimer = GetParent().GetParent().GetNode<Timer>("Agent/Timer");
		_notifier = GetParent().GetParent().GetNode<VisibleOnScreenNotifier2D>("VisibleOnScreenNotifier2D");
		_explodeEmitter = GetParent().GetParent().GetNode<GpuParticles2D>("ExplodeEmitter");
		_stateMachine = GetParent<Node2D>() as StateMachine;

		
		_hitbox = GetParent().GetParent().GetNode<Area2D>("Hitbox");
		_timer = 1.5f;
		
		_agentTimer.Timeout += OnAgentTimeout;
		
	}

	public override void Enter()
	{
		Logger.Log.Information(_parent.Name + " entering ram.");
		_agent.TargetPosition = _parent.Player.Position;
	}

	public override void Exit()
	{
	}

	public override void StateUpdate(double delta)
	{
		_parent.HandleAnimations();
	}

	public override void StatePhysicsUpdate(double delta)
	{
		if (!_notifier.IsOnScreen())
		{
			EmitSignal(SignalName.StateTransition, this, "sleep");
		}
		
		Vector2 goalVector = (_agent.GetNextPathPosition() - _parent.GlobalPosition).Normalized();
		if (!_parent.IsIdle)
			_parent.Rotation = Mathf.LerpAngle(_parent.Rotation, (goalVector.Angle() + Constants.HalfPiOffset), _parent.RotationSpeed / 60 );
		_parent.Velocity = new Vector2((float)(-_parent.Speed * Math.Sin(-_parent.Rotation)), (float)(-_parent.Speed * Math.Cos(-_parent.Rotation)));
		  
		if (_parent.Position.DistanceTo(_parent.Player.Position) <= 64)
		{
			_timer -= (float)delta;
			_parent.Velocity = Vector2.Zero;
			//Start flashing
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

		if (_parent.Position.DistanceTo(_parent.Player.Position) > 64)
		{
			_timer = 1.5f;
		}
		
		_parent.MoveAndSlide();
	}
	
	private void OnAgentTimeout()
	{
		if (_stateMachine.CurrentState == this)
			_agent.TargetPosition = _parent.Player.Position;
	}
}
