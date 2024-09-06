using BitBuster.world;
using Godot;
using Godot.Collections;

namespace BitBuster.entity.enemy;

public abstract partial class IdleEnemy: Enemy
{
    private CollisionShape2D _staticCollider;
    private StaticBody2D _staticBody;

    public override void _Ready()
    {
        base._Ready();
        _staticBody = GetNode<StaticBody2D>("StaticBody2D");
        _staticCollider = GetNode<CollisionShape2D>("StaticBody2D/CollisionShape2D");
    }


    protected void CleanAndRebake()
    {
        _staticBody.SetCollisionLayerValue((int)BbCollisionLayer.EntityNoPass, false);
        GlobalEvents.EmitBakeNavigationMeshSignal();
    }
}