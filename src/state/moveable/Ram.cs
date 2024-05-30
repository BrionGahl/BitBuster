using Godot;
using System;
using BitBuster.component;
using BitBuster.data;
using BitBuster.entity.enemy;
using BitBuster.state;
using BitBuster.utils;

public partial class Ram : State
{
	
	private MovingEnemy _parent;
	
	
	public override void Init()
	{
		_parent = GetParent().GetParent<CharacterBody2D>() as MovingEnemy;
	}

	public override void Enter()
	{
		Logger.Log.Information(_parent.Name + " entering ram.");
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
		if (!_parent.Notifier.IsOnScreen() || !_parent.Agent.IsTargetReachable())
		{
			EmitSignal(SignalName.StateTransition, this, "sleep");
		}
		
		_parent.AttackAction(delta);

		_parent.MoveAction(delta);
	}
}
