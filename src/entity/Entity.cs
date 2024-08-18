using BitBuster.resource;
using Godot;

namespace BitBuster.entity;

public abstract partial class Entity: CharacterBody2D
{
    private EntityStats _entityStats;
	
    [Export] 
    public EntityStats EntityStats
    {
        get => _entityStats;
        set => _entityStats = value;
    }
    
    protected CollisionShape2D Collider { get; private set; }
    protected GpuParticles2D ParticleDeath { get; private set; }
    protected Timer DeathAnimationTimer { get; private set; }


    public override void _Ready()
    {
        Collider = GetNode<CollisionShape2D>("Collider");
        DeathAnimationTimer = GetNode<Timer>("DeathAnimationTimer");
        ParticleDeath = GetNode<GpuParticles2D>("ParticleDeath");
        
        DeathAnimationTimer.Timeout += OnDeathAnimationTimeout;
    }
    
    protected abstract void OnDeathAnimationTimeout();
}