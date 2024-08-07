using BitBuster.projectile;
using BitBuster.utils;
using BitBuster.world;
using Godot;

namespace BitBuster.component;

public partial class OverhealBurstComponent : Area2D
{
	private GpuParticles2D _burstEmitter;
	private GpuParticles2D _burstInsideEmitter;
	private CollisionShape2D _areaCollider;

	private CircleShape2D _burst;
	private Timer _timer;
	
	public override void _Ready()
	{
		_burstEmitter = GetNode<GpuParticles2D>("BurstEmitter");
		_burstInsideEmitter = GetNode<GpuParticles2D>("BurstInsideEmitter");
		_areaCollider = GetNode<CollisionShape2D>("AreaCollider");

		_areaCollider.Shape = new CircleShape2D();
		_burst = (CircleShape2D)_areaCollider.Shape;
		_timer = GetNode<Timer>("Timer");

		_timer.Timeout += OnTimerTimeout;
	}

	public void Burst(float radius)
	{
		_burst.Radius = radius;
		((ParticleProcessMaterial)_burstEmitter.ProcessMaterial).EmissionRingRadius = radius;
		((ParticleProcessMaterial)_burstEmitter.ProcessMaterial).EmissionRingInnerRadius = radius;
		((ParticleProcessMaterial)_burstInsideEmitter.ProcessMaterial).EmissionSphereRadius = radius;
		
		_burstEmitter.Emitting = true;
		_burstInsideEmitter.Emitting = true;
		
		SetCollisionLayerValue((int)BBCollisionLayer.Projectile, true);

		_timer.Start();
	}

	private void OnTimerTimeout()
	{
		SetCollisionLayerValue((int)BBCollisionLayer.Projectile, false);
		_burstEmitter.Emitting = false;
		_burstInsideEmitter.Emitting = false;
	}
}
