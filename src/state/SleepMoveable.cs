using BitBuster.entity.enemy;
using Godot;

namespace BitBuster.state;

public partial class SleepMoveable: State
{

    private EnemyMoveable _parent;
    
    public override void Init()
    {
        _parent = GetParent().GetParent<CharacterBody2D>() as EnemyMoveable;
    }
    
    public override void Enter()
    {
        _parent.AgentTimer.Paused = true;
        _parent.Position = _parent.SpawnPosition;
    }

    public override void Exit()
    {
        _parent.AgentTimer.Paused = false;
    }

    public override void StateUpdate()
    {
    }

    public override void StatePhysicsUpdate()
    {
        if (_parent.OnScreenNotifier.IsOnScreen())
            EmitSignal(SignalName.StateTransition, this, "pursuemoveable");
    }
}