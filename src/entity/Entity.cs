using BitBuster.resource;
using Godot;

namespace BitBuster.entity;

public abstract partial class Entity: CharacterBody2D
{
    [Export] 
    public EntityStats EntityStats;
}