using System;
using BitBuster.entity.enemy;
using BitBuster.utils;
using Godot;

namespace BitBuster.state.moveable;

public partial class Evade: State
{
    private MovingEnemy _parent;
    
    public override void Init()
    {
        _parent = GetParent().GetParent<CharacterBody2D>() as MovingEnemy;
    }

    public override void Enter()
    {
        Logger.Log.Information(_parent.Name + " entering evade.");
       
        int x = (int) Math.Floor(_parent.Position.X / Constants.RoomSize) * Constants.RoomSize;
        int y = (int) Math.Floor(_parent.Position.Y / Constants.RoomSize) * Constants.RoomSize;
        
        _parent.Target = new Vector2(x + Constants.RoomSize - (_parent.Player.Position.X % Constants.RoomSize), y + Constants.RoomSize - (_parent.Player.Position.Y % Constants.RoomSize));
    }

    public override void Exit()
    {
        _parent.Target = Vector2.Zero;
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

        if (_parent.Position.DistanceTo(_parent.Target) < 32)
        {
            EmitSignal(SignalName.StateTransition, this, "pursue");
        }
		
        _parent.AttackAction(delta);

        _parent.MoveAction(delta);
    }
}