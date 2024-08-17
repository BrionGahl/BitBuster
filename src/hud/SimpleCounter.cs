using BitBuster.component;
using BitBuster.entity.player;
using BitBuster.resource;
using Godot;

namespace BitBuster.hud;

public abstract partial class SimpleCounter: HSplitContainer
{
    protected EntityStats PlayerStats;
    protected Label Counter;
	
    public override void _Ready()
    {
        PlayerStats = ((Player)GetTree().GetFirstNodeInGroup("player")).EntityStats;
        Counter = GetNode<Label>("Counter");
        
        PlayerStats.StatChange += OnStatChange;
        PlayerStats.EmitStatChangeSignal();
    }

    protected abstract void OnStatChange();
}