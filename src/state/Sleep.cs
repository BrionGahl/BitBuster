using BitBuster.entity.enemy;
using BitBuster.entity.player;
using BitBuster.utils;
using Godot;

namespace BitBuster.state;

public partial class Sleep: State
{
	[Export]
	public string NextState;

	public override void Init(Enemy enemy)
	{
		ParentEnemy = enemy;
	}
	
	public override void Enter()
	{
		Logger.Log.Information(ParentEnemy.EntityStats.Name + " entering sleep.");
		
		if (ParentEnemy is MovingEnemy movingEnemy)
			movingEnemy.AgentTimer.Paused = true;

		ParentEnemy.Position = ParentEnemy.SpawnPosition;
	}

	public override void Exit()
	{
		if (ParentEnemy is MovingEnemy movingEnemy)
			movingEnemy.AgentTimer.Paused = false;
		
		ParentEnemy.Target = Vector2.Zero;
	}

	public override void StateUpdate(double delta)
	{
	}

	public override void StatePhysicsUpdate(double delta)
	{

		if (!ParentEnemy.Notifier.IsOnScreen())
			return;
		
		if (ParentEnemy is MovingEnemy movingEnemy)
		{
			movingEnemy.Agent.TargetPosition = ParentEnemy.Player.GlobalPosition;
			if (movingEnemy.Agent.IsTargetReachable()) 
				EmitSignal(SignalName.StateTransition, this, NextState);
			return;
		}
		
		EmitSignal(State.SignalName.StateTransition, this, NextState);
	}
}
