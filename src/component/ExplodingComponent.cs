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
	private CircleShape2D _explosion;
	
	public override void _Ready()
	{
		_globalEvents = GetNode<GlobalEvents>("/root/GlobalEvents");
		_explodingEmitter = GetNode<GpuParticles2D>("ExplodeEmitter");
		_explosion = GetNode<CollisionShape2D>("AreaCollider").Shape as CircleShape2D;


		if (StatsComponent == null)
		{
			_explosion.Radius = 25f;
			((ParticleProcessMaterial)_explodingEmitter.ProcessMaterial).EmissionSphereRadius = 25f;
			return;
		}
		
		_explosion.Radius = StatsComponent.BombRadius;
		((ParticleProcessMaterial)_explodingEmitter.ProcessMaterial).EmissionSphereRadius = StatsComponent.BombRadius;
	}

	public void Explode(float radius, AttackData attackData)
	{
		_explodingEmitter.Emitting = true;
		
		Logger.Log.Information("Length of Areas: " + GetOverlappingAreas().Count);
		foreach (var area in GetOverlappingAreas())
		{
			Logger.Log.Information("Area Name: " + area.GetParent().Name);
			if (area is HitboxComponent hitboxComponent)
			{
				Logger.Log.Information("Hitbox hit at " + hitboxComponent.Name);

				hitboxComponent.Damage(attackData);
			}
		}
			
		
		Logger.Log.Information("Length of Bodies: " + GetOverlappingBodies().Count);
		foreach (var body in GetOverlappingBodies())
		{
			Logger.Log.Information("Body Name: " + body.Name);
			if (!body.IsInGroup(Groups.GroupBreakable)) 
				continue;
			Logger.Log.Information("Hit Wall...");
			((BreakableWall)body).Break();
		}
		_globalEvents.EmitBakeNavigationMeshSignal();
	}
}
