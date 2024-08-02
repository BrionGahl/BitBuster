using BitBuster.world;
using Godot;
using Godot.Collections;

namespace BitBuster.entity.enemy;

public abstract partial class IdleEnemy: Enemy
{
    private GlobalEvents _globalEvents;
    private CollisionShape2D _staticCollider;
    private StaticBody2D _staticBody;

    public override void _Ready()
    {
        base._Ready();
        _globalEvents = GetNode<GlobalEvents>("/root/GlobalEvents");

        _staticBody = GetNode<StaticBody2D>("StaticBody2D");
        _staticCollider = GetNode<CollisionShape2D>("StaticBody2D/CollisionShape2D");
    }

    
    public void CleanAndRebake()
    {
        _staticBody.SetCollisionLayerValue((int)BBCollisionLayer.EntityNoPass, false);
        _globalEvents.EmitBakeNavigationMeshSignal();
    }
}