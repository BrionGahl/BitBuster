using Godot;
using System;
using BitBuster.entity.enemy;
using BitBuster.state;
using BitBuster.utils;

public partial class Attack : State
{
	
	private Enemy _parent;
	
	public override void Init()
	{
		_parent = GetParent().GetParent<CharacterBody2D>() as Enemy;
	}

	public override void Enter()
	{
		Logger.Log.Information(_parent.Name + " entering scan.");
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
		if (!_parent.Notifier.IsOnScreen())
		{
			EmitSignal(SignalName.StateTransition, this, "sleep");
		}

		_parent.AttackAction(delta);
	}
}
