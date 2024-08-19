using BitBuster.entity.enemy;
using BitBuster.entity.enemy.moving;
using BitBuster.utils;
using Godot;

namespace BitBuster.state.moveable;

public partial class Ram : State
{
	
	public override void Init(Enemy enemy)
	{
		ParentEnemy = enemy;
	}

	public override void Enter()
	{
		Logger.Log.Information(ParentEnemy.EntityStats.Name + " entering ram.");
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
		if (!ParentEnemy.Notifier.IsOnScreen() || !((MovingEnemy)ParentEnemy).Agent.IsTargetReachable())
		{
			EmitSignal(State.SignalName.StateTransition, this, "sleep");
		}
		
		if (ParentEnemy.Position.DistanceTo(ParentEnemy.Player.Position) < 48 && ParentEnemy is not TankDetonator)
		{
			EmitSignal(State.SignalName.StateTransition, this, "evade");
		}
		
		ParentEnemy.AttackAction(delta);
		((MovingEnemy)ParentEnemy).MoveAction(delta);
	}
}
