using BitBuster.component;
using BitBuster.entity.enemy;
using BitBuster.utils;
using Godot;

namespace BitBuster.state;

public partial class Sleep: State
{

	[Export]
	public string NextState;
	
	private Enemy _parent;
	private Timer _agentTimer;
	private NavigationAgent2D _agent;
	
	public override void Init()
	{
		_parent = GetParent().GetParent<CharacterBody2D>() as Enemy;

		_agentTimer = GetParent().GetParent().GetNodeOrNull<Timer>("Agent/Timer");
		_agent = GetParent().GetParent().GetNodeOrNull<NavigationAgent2D>("Agent");
	}
	
	public override void Enter()
	{
		Logger.Log.Information(_parent.Name + " entering sleep.");
		if (_parent is MovingEnemy)
			_agentTimer.Paused = true;

		_parent.Position = _parent.SpawnPosition;
	}

	public override void Exit()
	{
		if (_parent is MovingEnemy)
			_agentTimer.Paused = false;
		
		_parent.Target = Vector2.Zero;

	}

	public override void StateUpdate(double delta)
	{
	}

	public override void StatePhysicsUpdate(double delta)
	{

		if (_parent is MovingEnemy)
		{
			MovingEnemy movingEnemy = _parent as MovingEnemy;
			if (movingEnemy.Notifier.IsOnScreen() && _agent.IsTargetReachable()) 
				EmitSignal(SignalName.StateTransition, this, NextState);
		}
		
		
		if (_parent.Notifier.IsOnScreen()) 
			EmitSignal(SignalName.StateTransition, this, NextState);
	}
}
