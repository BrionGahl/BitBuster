using BitBuster.entity.enemy;
using Godot;

namespace BitBuster.state;

public partial class Sleep: State
{
    private Enemy _parent;

    public override void Init()
    {
        _parent = GetParent().GetParent<CharacterBody2D>() as EnemyMoveable;
    }

    public override void Enter()
    {
    }

    public override void Exit()
    {
    }

    public override void StateUpdate()
    {
    }

    public override void StatePhysicsUpdate()
    {
        if (_parent.OnScreenNotifier.IsOnScreen())
            EmitSignal(SignalName.StateTransition, this, "attack");
    }
}