using BitBuster.entity.enemy;
using Godot;

namespace BitBuster.state;

public abstract partial class State : Node
{
    [Signal]
    public delegate void StateTransitionEventHandler(State state, string newStateName);

    protected Enemy ParentEnemy { get; set; }
    
    public abstract void Init(Enemy enemy);
    public abstract void Enter();
    public abstract void Exit();
    public abstract void StateUpdate(double delta);
    public abstract void StatePhysicsUpdate(double delta);
}