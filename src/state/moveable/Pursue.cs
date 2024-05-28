using System;
using BitBuster.entity.enemy;
using BitBuster.entity.player;
using BitBuster.utils;
using Godot;

namespace BitBuster.state.moveable;


public partial class Pursue: State
{
	private MovingEnemy _parent;
	
	public override void Init()
	{
		_parent = GetParent().GetParent<CharacterBody2D>() as MovingEnemy;
	}
	
	public override void Enter()
	{
		Logger.Log.Information(_parent.Name + " entering pursue.");
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

		if (_parent.CanSeePlayer() && _parent.Position.DistanceTo(_parent.Player.Position) > 64)
		{
			EmitSignal(SignalName.StateTransition, this, "pace");
		}
	
		if (_parent.Position.DistanceTo(_parent.Player.Position) < 64) // Enter Evade
		{
			EmitSignal(SignalName.StateTransition, this, "evade");
		}
		  
		_parent.AttackAction(delta);

		_parent.MoveAction(delta);
	}
}
