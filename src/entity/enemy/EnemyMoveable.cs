using BitBuster.state;
using Godot;

namespace BitBuster.entity.enemy;

public abstract partial class EnemyMoveable: Enemy
{
    public NavigationAgent2D Agent { get; set; }
    public Timer AgentTimer { get; set; }

    public override void _Ready()
    {
        base._Ready();
        
        Agent = GetNode<NavigationAgent2D>("Agent");
        AgentTimer = GetNode<Timer>("Agent/Timer");
    }
}