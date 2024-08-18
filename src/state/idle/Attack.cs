using BitBuster.entity;
using BitBuster.entity.enemy;
using BitBuster.utils;
using Godot;

namespace BitBuster.state.idle;

public partial class Attack : State
{
	
	public override void Init(Enemy enemy)
	{
		ParentEnemy = enemy;
	}

	public override void Enter()
	{
		Logger.Log.Information(ParentEnemy.Name + " entering scan.");
	}

	public override void Exit()
	{
	}

	public override void StateUpdate(double delta)
	{
		ParentEnemy.HandleAnimations();
	}

	public override void StatePhysicsUpdate(double delta)
	{
		if (!ParentEnemy.Notifier.IsOnScreen())
		{
			EmitSignal(State.SignalName.StateTransition, this, "sleep");
		}

		ParentEnemy.AttackAction(delta);
	}
}
