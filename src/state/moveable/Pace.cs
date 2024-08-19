using System;
using BitBuster.entity;
using BitBuster.entity.enemy;
using BitBuster.utils;
using Godot;

namespace BitBuster.state.moveable;

public partial class Pace: State
{
    private RandomNumberGenerator _randomNumberGenerator;
    private Vector2 _paceStartPosition;
    
    public override void Init(Enemy enemy)
    {
        ParentEnemy = enemy;
        
        _randomNumberGenerator = new RandomNumberGenerator();
        _randomNumberGenerator.Randomize();
    }

    public override void Enter()
    {
        Logger.Log.Information(ParentEnemy.EntityStats.Name + " entering pace.");
        _paceStartPosition = ParentEnemy.Position;
        GetNewTarget();
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

        if (!ParentEnemy.CanSeePlayer())
        {
            EmitSignal(State.SignalName.StateTransition, this, "pursue");
        }
	
        if (ParentEnemy.Position.DistanceTo(ParentEnemy.Player.Position) < 64) 
        {
            EmitSignal(State.SignalName.StateTransition, this, "evade");
        }
        
        if (ParentEnemy.Position.DistanceTo(ParentEnemy.Target) < 16)
        {
            GetNewTarget();
        }
        
        ParentEnemy.AttackAction(delta);

        ((MovingEnemy)ParentEnemy).MoveAction(delta);
        
    }

    private void GetNewTarget()
    {
        ParentEnemy.Target = new Vector2(_paceStartPosition.X + _randomNumberGenerator.RandiRange(-32, 32), _paceStartPosition.Y + _randomNumberGenerator.RandiRange(-32, 32));
    }
    
}