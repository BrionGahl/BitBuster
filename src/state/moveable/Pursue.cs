using BitBuster.entity.enemy;
using BitBuster.utils;

namespace BitBuster.state.moveable;

public partial class Pursue: State
{
	public override void Init(Enemy enemy)
	{
		ParentEnemy = enemy;
	}
	
	public override void Enter()
	{
		Logger.Log.Information(ParentEnemy.EntityStats.Name + " entering pursue.");
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

		if (ParentEnemy.CanSeePlayer() && ParentEnemy.Position.DistanceTo(ParentEnemy.Player.Position) > 64)
		{
			EmitSignal(State.SignalName.StateTransition, this, "pace");
		}
	
		if (ParentEnemy.Position.DistanceTo(ParentEnemy.Player.Position) < 64)
		{
			EmitSignal(State.SignalName.StateTransition, this, "evade");
		}
		  
		ParentEnemy.AttackAction(delta);
		((MovingEnemy)ParentEnemy).MoveAction(delta);
	}
}
