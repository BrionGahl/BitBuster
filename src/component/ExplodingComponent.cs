using BitBuster.data;
using BitBuster.tiles;
using BitBuster.utils;
using BitBuster.world;
using Godot;

namespace BitBuster.component;

public partial class ExplodingComponent : Area2D
{
	[Export]
	public StatsComponent StatsComponent { get; set; }
	
	private GlobalEvents _globalEvents;
	private GpuParticles2D _explodingEmitter;
	private CollisionShape2D _areaCollider;
	
	private CircleShape2D _explosion;
	
	public override void _Ready()
	{
		_globalEvents = GetNode<GlobalEvents>("/root/GlobalEvents");
		_explodingEmitter = GetNode<GpuParticles2D>("ExplodeEmitter");
		_areaCollider = GetNode<CollisionShape2D>("AreaCollider");
		
		_areaCollider.Shape = new CircleShape2D();
		_explosion = (CircleShape2D)_areaCollider.Shape;
		
		if (GetParent().IsInGroup(Groups.GroupBullet))
		{
			_explosion.Radius = 20f;
			((ParticleProcessMaterial)_explodingEmitter.ProcessMaterial).EmissionSphereRadius = 25f;
			return;
		}
		
		_explosion.Radius = StatsComponent.BombRadius;
		((ParticleProcessMaterial)_explodingEmitter.ProcessMaterial).EmissionSphereRadius = StatsComponent.BombRadius;
	}

	public void Explode(AttackData attackData)
	{
		_explodingEmitter.Emitting = true;
		foreach (var area in GetOverlappingAreas())
		{
			if (area is HitboxComponent hitboxComponent)
			{
				hitboxComponent.Damage(attackData);
			}
		}
		foreach (var body in GetOverlappingBodies())
		{
			if (!body.IsInGroup(Groups.GroupBreakable)) 
				continue;
			((BreakableWall)body).Break();
		}
		_globalEvents.EmitBakeNavigationMeshSignal();
	}
}
