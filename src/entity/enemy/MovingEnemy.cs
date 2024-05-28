using Godot;

namespace BitBuster.entity.enemy;

public abstract partial class MovingEnemy: Enemy
{
    public NavigationAgent2D Agent;
    public Timer AgentTimer;
    
    public override void _Ready()
    {
        base._Ready();

        Agent = GetNode<NavigationAgent2D>("Agent");
        AgentTimer = GetNode<Timer>("Agent/Timer");
    }
    
    public abstract void MoveAction(double delta);
    public abstract void OnAgentTimeout();
}