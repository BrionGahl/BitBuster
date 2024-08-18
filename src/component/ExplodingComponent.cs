using BitBuster.data;
using BitBuster.resource;
using BitBuster.tiles;
using BitBuster.utils;
using BitBuster.world;
using Godot;

namespace BitBuster.component;

public partial class ExplodingComponent : Area2D
{
	private GlobalEvents _globalEvents;
	private CollisionShape2D _areaCollider;
	private CircleShape2D _explosion;
	private EntityStats _entityStats;
	
	public EntityStats EntityStats
	{
		get => _entityStats;
		set
		{
			_entityStats = value;
			
			_explosion.Radius = _entityStats.BombRadius;
			((ParticleProcessMaterial)ExplodingEmitter.ProcessMaterial).EmissionSphereRadius = _entityStats.BombRadius;
		}
	}

	public GpuParticles2D ExplodingEmitter { get; private set; }


	public override void _Ready()
	{
		_globalEvents = GetNode<GlobalEvents>("/root/GlobalEvents");
		
		ExplodingEmitter = GetNode<GpuParticles2D>("ExplodeEmitter");
		_areaCollider = GetNode<CollisionShape2D>("AreaCollider");
		
		_areaCollider.Shape = new CircleShape2D();
		_explosion = (CircleShape2D)_areaCollider.Shape;
		
		_explosion.Radius = 20f;
		((ParticleProcessMaterial)ExplodingEmitter.ProcessMaterial).EmissionSphereRadius = 25f;

		ExplodingEmitter.Finished += OnExplodeFinished;
	}

	public void Explode(AttackData attackData)
	{
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
		ExplodingEmitter.Emitting = true;
	}

	private void OnExplodeFinished()
	{
		_globalEvents.EmitBakeNavigationMeshSignal();
	}
}
