using System;
using BitBuster.entity.enemy;
using BitBuster.utils;
using Godot;

namespace BitBuster.state.moveable;

public partial class Evade: State
{
    private Enemy _parent;
    private NavigationAgent2D _agent;
    private Timer _agentTimer;
    private VisibleOnScreenNotifier2D _notifier;
    private StateMachine _stateMachine;
	
    private RandomNumberGenerator _randomNumberGenerator;

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
        Logger.Log.Information(_parent.Name + " entering evade.");
       
        int x = (int) Math.Floor(_parent.Position.X / Constants.RoomSize) * Constants.RoomSize;
        int y = (int) Math.Floor(_parent.Position.Y / Constants.RoomSize) * Constants.RoomSize;
        
        _target = new Vector2(x + Constants.RoomSize - (_parent.Player.Position.X % Constants.RoomSize), y + Constants.RoomSize - (_parent.Player.Position.Y % Constants.RoomSize));
        _agent.TargetPosition = _target;
    }

    public override void Exit()
    {
    }

    public override void StateUpdate()
    {
        _parent.HandleAnimations();
    }

    public override void StatePhysicsUpdate()
    {
        if (!_notifier.IsOnScreen())
        {
            EmitSignal(SignalName.StateTransition, this, "sleep");
        }

        if (_agent.DistanceToTarget() < 32)
        {
            EmitSignal(SignalName.StateTransition, this, "pursue");
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
    
    private void OnAgentTimeout()
    {
        if (_stateMachine.CurrentState == this)
            _agent.TargetPosition = _target;
    }
}