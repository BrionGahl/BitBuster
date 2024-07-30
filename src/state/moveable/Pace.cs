using System;
using BitBuster.entity.enemy;
using BitBuster.utils;
using Godot;

namespace BitBuster.state.moveable;

public partial class Pace: State
{
    private MovingEnemy _parent;
    
    private RandomNumberGenerator _randomNumberGenerator;

    private Vector2 _paceStartPosition;
    
    public override void Init()
    {
        _parent = GetParent().GetParent<CharacterBody2D>() as MovingEnemy;
        
        _randomNumberGenerator = new RandomNumberGenerator();
        _randomNumberGenerator.Randomize();
    }

    public override void Enter()
    {
        Logger.Log.Information(_parent.Name + " entering pace.");
        _paceStartPosition = _parent.Position;
        GetNewTarget();
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

        if (!_parent.CanSeePlayer())
        {
            EmitSignal(SignalName.StateTransition, this, "pursue");
        }
	
        if (_parent.Position.DistanceTo(_parent.Player.Position) < 64) 
        {
            EmitSignal(SignalName.StateTransition, this, "evade");
        }
        
        if (_parent.Position.DistanceTo(_parent.Target) < 16)
        {
            GetNewTarget();
        }
        
        _parent.AttackAction(delta);

        _parent.MoveAction(delta);
        
    }

    private void GetNewTarget()
    {
        _parent.Target = new Vector2(_paceStartPosition.X + _randomNumberGenerator.RandiRange(-32, 32), _paceStartPosition.Y + _randomNumberGenerator.RandiRange(-32, 32));
    }
    
}