using System;
using BitBuster.entity.enemy;
using BitBuster.entity.player;
using BitBuster.utils;
using Godot;

namespace BitBuster.state;

// public partial class PursueMoveable: State
// {
//     private EnemyMoveable _parent;
//     private RandomNumberGenerator _randomNumberGenerator;
//     
    // public override void Init()
    // {
    //     _parent = GetParent().GetParent<CharacterBody2D>() as EnemyMoveable;
    //     
    //     Logger.Log.Information(_parent + "124");
    //     
    //     _randomNumberGenerator = new RandomNumberGenerator();
    //     _randomNumberGenerator.Randomize();
    //     
    //     _parent.AgentTimer.Timeout += OnAgentTimeout;
    // }
    //
    // public override void Enter()
    // {
    // }
    //
    // public override void Exit()
    // {
    // }
    //
    // public override void StateUpdate()
    // {
    //     _parent.HandleAnimations();
    // }
    //
    // public override void StatePhysicsUpdate()
    // {
    //     if (!_parent.Agent.IsTargetReachable())
    //     {
    //         EmitSignal(SignalName.StateTransition, this, "sleepmoveable");
    //     }
    //
    //     if (_parent.Agent.DistanceToTarget() < 64) // Enter Evade
    //     {
    //         
    //     }
		  //
    //     _parent.SetGunRotationAndPosition();
    //     if (_parent.CanSeePlayer() && _randomNumberGenerator.Randf() > 0.3f)
    //         _parent.WeaponComponent.AttemptShoot(_parent.Player.Position.AngleToPoint(_parent.Position) + _randomNumberGenerator.RandfRange(-Mathf.Pi / 9, Mathf.Pi / 9));
		  //
    //     Vector2 goalVector = (_parent.Agent.GetNextPathPosition() - _parent.GlobalPosition).Normalized();
    //     if (!_parent.IsIdle)
    //         _parent.Rotation = Mathf.LerpAngle(_parent.Rotation, (goalVector.Angle() + Constants.HalfPiOffset), _parent.RotationSpeed / 60 );
    //     _parent.Velocity = new Vector2((float)(-_parent.Speed * Math.Sin(-_parent.Rotation)), (float)(-_parent.Speed * Math.Cos(-_parent.Rotation)));
		  //
    //     _parent.MoveAndSlide();    }
    //
    // private void OnAgentTimeout()
    // {
    //     if (_parent.StateMachine.CurrentState == this) {}
    //         _parent.Agent.TargetPosition = _parent.Player.Position;
    //
    // }
// }