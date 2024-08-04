using BitBuster.data;
using BitBuster.tiles;
using BitBuster.utils;
using BitBuster.world;
using Godot;

namespace BitBuster.component;

public partial class ExplodingComponent : Area2D
{
	private GlobalEvents _globalEvents;
	private GpuParticles2D _explodingEmitter;
	private CircleShape2D _explosion;
	
	public override void _Ready()
	{
		_globalEvents = GetNode<GlobalEvents>("/root/GlobalEvents");
		_explodingEmitter = GetNode<GpuParticles2D>("ExplodeEmitter");
		_explosion = GetNode<CollisionShape2D>("AreaCollider").Shape as CircleShape2D;
	}

	public void Explode(float radius, AttackData attackData)
	{
		_explosion.Radius = radius;
		(_explodingEmitter.ProcessMaterial as ParticleProcessMaterial).EmissionSphereRadius = radius;

		_explodingEmitter.Emitting = true;
			
		foreach (var area in GetOverlappingAreas())
		{
			if (area is HitboxComponent)
			{
				Logger.Log.Information("Hitbox hit at " + area.Name);

				HitboxComponent hitboxComponent = area as HitboxComponent;
				hitboxComponent.Damage(attackData);
			}
		}
			
		foreach (var body in GetOverlappingBodies())
		{
			if (!body.IsInGroup(Groups.GroupBreakable)) 
				continue;
			Logger.Log.Information("Hit Wall...");
			(body as BreakableWall).Break();
		}
		_globalEvents.EmitBakeNavigationMeshSignal();
	}
}
