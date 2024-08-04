using Godot;
using System;
using BitBuster.projectile;
using BitBuster.utils;
using BitBuster.world;

public partial class OverhealBurstComponent : Area2D
{
	private GpuParticles2D _burstEmitter;
	private CircleShape2D _burst;
	private Timer _timer;
	
	public override void _Ready()
	{
		_burstEmitter = GetNode<GpuParticles2D>("BurstEmitter");
		_burst = GetNode<CollisionShape2D>("AreaCollider").Shape as CircleShape2D;
		_timer = GetNode<Timer>("Timer");

		_timer.Timeout += OnTimerTimeout;
	}

	public void Burst(float radius)
	{
		_burst.Radius = radius;
		(_burstEmitter.ProcessMaterial as ParticleProcessMaterial).EmissionRingRadius = radius;
		(_burstEmitter.ProcessMaterial as ParticleProcessMaterial).EmissionRingInnerRadius = radius;

		_burstEmitter.Emitting = true;

		SetCollisionLayerValue((int)BBCollisionLayer.Projectile, true);
		
		foreach (var body in GetOverlappingBodies())
			if (body.IsInGroup(Groups.GroupBullet) && !body.GetParent().GetParent().IsInGroup(Groups.GroupPlayer))
				(body as Bullet).PrepForFree();
		
		_timer.Start();
	}

	private void OnTimerTimeout()
	{
		SetCollisionLayerValue((int)BBCollisionLayer.Projectile, false);
		_burstEmitter.Emitting = false;
	}
}
