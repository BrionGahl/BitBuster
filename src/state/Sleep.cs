using BitBuster.component;
using BitBuster.entity.enemy;
using BitBuster.entity.player;
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
	private Player _player;

	public override void Init()
	{
		_parent = GetParent().GetParent<CharacterBody2D>() as Enemy;

		_agentTimer = GetParent().GetParent().GetNodeOrNull<Timer>("Agent/Timer");
		_agent = GetParent().GetParent().GetNodeOrNull<NavigationAgent2D>("Agent");
		_player = GetTree().GetFirstNodeInGroup("player") as Player;
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
			_agent.TargetPosition = _player.GlobalPosition;
			MovingEnemy movingEnemy = _parent as MovingEnemy;
			if (movingEnemy.Notifier.IsOnScreen() && _agent.IsTargetReachable()) 
				EmitSignal(SignalName.StateTransition, this, NextState);
			return;
		}
		
		if (_parent.Notifier.IsOnScreen()) 
			EmitSignal(SignalName.StateTransition, this, NextState);
	}
}
