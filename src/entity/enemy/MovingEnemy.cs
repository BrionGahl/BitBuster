using Godot;

namespace BitBuster.entity.enemy;

public abstract partial class MovingEnemy: Enemy
{
    protected NavigationAgent2D Agent;
    protected Timer AgentTimer;
    
    public override void _Ready()
    {
        base._Ready();

        Agent = GetNode<NavigationAgent2D>("Agent");
        AgentTimer = GetNode<Timer>("Agent/Timer");
    }
    
    public abstract void MoveAction(double delta);
    public abstract void OnAgentTimeout();
}