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
	
	public override void Init()
	{
		_parent = GetParent().GetParent<CharacterBody2D>() as Enemy;

		_agentTimer = GetParent().GetParent().GetNodeOrNull<Timer>("Agent/Timer");
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
	}

	public override void StateUpdate(double delta)
	{
	}

	public override void StatePhysicsUpdate(double delta)
	{
		if (_parent.Notifier.IsOnScreen()) 
			EmitSignal(SignalName.StateTransition, this, NextState);
	}
}
