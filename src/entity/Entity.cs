using BitBuster.resource;
using Godot;

namespace BitBuster.entity;

public abstract partial class Entity: CharacterBody2D
{
    [Export] 
    public EntityStats EntityStats { get; set; }
    protected CollisionShape2D Collider { get; private set; }
    protected GpuParticles2D ParticleDeath { get; private set; }

    public override void _Ready()
    {
        Collider = GetNode<CollisionShape2D>("Collider");
        ParticleDeath = GetNode<GpuParticles2D>("ParticleDeath");
        
        ParticleDeath.Finished += OnParticleDeathFinished;
    }
    
    protected abstract void OnParticleDeathFinished();
}