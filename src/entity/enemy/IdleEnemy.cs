using BitBuster.world;
using Godot;

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
        _staticCollider.Disabled = true;
        _globalEvents.EmitBakeNavigationMeshSignal(Vector2.Inf);
    }
}