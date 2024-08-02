using BitBuster.world;
using Godot;
using Godot.Collections;

namespace BitBuster.entity.enemy;

public abstract partial class IdleEnemy: Enemy
{
    private GlobalEvents _globalEvents;
    private CollisionShape2D _staticCollider;

    public override void _Ready()
    {
        base._Ready();
        _globalEvents = GetNode<GlobalEvents>("/root/GlobalEvents");

        _staticCollider = GetNode<CollisionShape2D>("StaticBody2D/CollisionShape2D");
    }

    public void CleanAndRebake()
    {
        _staticCollider.SetDeferred("Disabled", true);
        _globalEvents.EmitBakeNavigationMeshSignal();
    }
}