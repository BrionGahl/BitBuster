using BitBuster.component;
using BitBuster.entity.enemy;
using BitBuster.utils;
using Godot;

namespace BitBuster.state;

public partial class Sleep: State
{

	private Enemy _parent;
	private Timer _agentTimer;
	private VisibleOnScreenNotifier2D _notifier;
	
	public override void Init()
	{
		_parent = GetParent().GetParent<CharacterBody2D>() as Enemy;
		_agentTimer = GetParent().GetParent().GetNodeOrNull<Timer>("Agent/Timer");
		_notifier = GetParent().GetParent().GetNode<VisibleOnScreenNotifier2D>("VisibleOnScreenNotifier2D");
	}
	
	public override void Enter()
	{
		Logger.Log.Information(_parent.Name + " entering sleep.");
		if (_agentTimer != null)
			_agentTimer.Paused = true;
		_parent.Position = _parent.SpawnPosition;
	}

	public override void Exit()
	{
		if (_agentTimer != null)
			_agentTimer.Paused = false;
	}

	public override void StateUpdate(double delta)
	{
	}

	public override void StatePhysicsUpdate(double delta)
	{
		if (_notifier.IsOnScreen())
			if (_agentTimer != null && _parent.WeaponComponent != null)
				EmitSignal(SignalName.StateTransition, this, "pursue");
			else if (_agentTimer != null)
				EmitSignal(SignalName.StateTransition, this, "ram");
			else
				EmitSignal(SignalName.StateTransition, this, "attack");
	}
}
