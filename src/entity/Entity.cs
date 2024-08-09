using BitBuster.resource;
using Godot;

namespace BitBuster.entity;

public abstract partial class Entity: CharacterBody2D
{
    [Export] 
    public EntityStats EntityStats; 
    
    protected CollisionShape2D Collider { get; private set; }


    public override void _Ready()
    {
        Collider = GetNode<CollisionShape2D>("Collider");
    }
}