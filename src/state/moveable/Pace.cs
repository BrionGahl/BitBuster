using System;
using BitBuster.entity.enemy;
using BitBuster.utils;
using Godot;

namespace BitBuster.state.moveable;

public partial class Pace: State
{
    private Enemy _parent;
    private NavigationAgent2D _agent;
    private Timer _agentTimer;
    private VisibleOnScreenNotifier2D _notifier;
    private StateMachine _stateMachine;
	
    private RandomNumberGenerator _randomNumberGenerator;

    private Vector2 _paceStartPosition;
    private Vector2 _target;
    
    public override void Init()
    {
        _parent = GetParent().GetParent<CharacterBody2D>() as Enemy;
        _agent = GetParent().GetParent().GetNode<NavigationAgent2D>("Agent");
        _agentTimer = GetParent().GetParent().GetNode<Timer>("Agent/Timer");
        _notifier = GetParent().GetParent().GetNode<VisibleOnScreenNotifier2D>("VisibleOnScreenNotifier2D");
        _stateMachine = GetParent<Node2D>() as StateMachine;
		
        _randomNumberGenerator = new RandomNumberGenerator();
        _randomNumberGenerator.Randomize();    
        _agentTimer.Timeout += OnAgentTimeout;

    }

    public override void Enter()
    {
        Logger.Log.Information(_parent.Name + " entering pace.");
        _paceStartPosition = _parent.Position;
        GetNewTarget();
        _agent.TargetPosition = _target;
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
        if (!_notifier.IsOnScreen())
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
        
        if (_agent.DistanceToTarget() < 16)
        {
            GetNewTarget();
        }
		  
        _parent.SetGunRotationAndPosition(Mathf.Pi/12);
        if (_parent.CanSeePlayer() && _randomNumberGenerator.Randf() > 0.3f)
            _parent.WeaponComponent.AttemptShoot(_parent.Player.Position.AngleToPoint(_parent.Position) + _randomNumberGenerator.RandfRange(-Mathf.Pi / 9, Mathf.Pi / 9));
		  
        Vector2 goalVector = (_agent.GetNextPathPosition() - _parent.GlobalPosition).Normalized();
        if (!_parent.IsIdle)
            _parent.Rotation = Mathf.LerpAngle(_parent.Rotation, (goalVector.Angle() + Constants.HalfPiOffset), _parent.RotationSpeed / 60 );
        _parent.Velocity = new Vector2((float)(-_parent.Speed * Math.Sin(-_parent.Rotation)), (float)(-_parent.Speed * Math.Cos(-_parent.Rotation)));
		  
        _parent.MoveAndSlide();
        
    }

    private void GetNewTarget()
    {
        _target = new Vector2(_paceStartPosition.X + _randomNumberGenerator.RandiRange(-16, 16), _paceStartPosition.Y + _randomNumberGenerator.RandiRange(-16, 16));
        
        
    }

    private void OnAgentTimeout()
    {
        if (_stateMachine.CurrentState == this)
        {
            _agent.TargetPosition = _target;
            if (!_agent.IsTargetReachable())
            {
                GetNewTarget();
                OnAgentTimeout();
            }
        }
    }
}