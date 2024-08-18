using System;
using BitBuster.entity;
using BitBuster.entity.enemy;
using BitBuster.utils;
using Godot;

namespace BitBuster.state.moveable;

public partial class Evade: State
{
    [Export]
    public string NextState = "pursue";
    
    public override void Init(Enemy enemy)
    {
        ParentEnemy = enemy;
    }

    public override void Enter()
    {
        Logger.Log.Information(ParentEnemy.EntityStats.Name + " entering evade.");
       
        int x = (int) Math.Floor(ParentEnemy.Position.X / Constants.RoomSize) * Constants.RoomSize;
        int y = (int) Math.Floor(ParentEnemy.Position.Y / Constants.RoomSize) * Constants.RoomSize;
        
        ParentEnemy.Target = new Vector2(x + Constants.RoomSize - (ParentEnemy.Player.Position.X % Constants.RoomSize), y + Constants.RoomSize - (ParentEnemy.Player.Position.Y % Constants.RoomSize));
    }

    public override void Exit()
    {
        ParentEnemy.Target = Vector2.Zero;
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

        if (ParentEnemy.Position.DistanceTo(ParentEnemy.Target) < 32)
        {
            EmitSignal(State.SignalName.StateTransition, this, NextState);
        }
		
        ParentEnemy.AttackAction(delta);
        ((MovingEnemy)ParentEnemy).MoveAction(delta);
    }
}