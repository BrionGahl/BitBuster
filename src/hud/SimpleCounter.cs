using BitBuster.component;
using Godot;

namespace BitBuster.hud;

public abstract partial class SimpleCounter: HSplitContainer
{
    protected StatsComponent PlayerStats;
    protected Label Counter;
	
    public override void _Ready()
    {
        PlayerStats = GetTree().GetFirstNodeInGroup("player").GetNode("StatsComponent") as StatsComponent;
        Counter = GetNode<Label>("Counter");
        
        PlayerStats.StatChange += OnStatChange;
        PlayerStats.EmitStatChangeSignal();
    }

    protected abstract void OnStatChange();
}