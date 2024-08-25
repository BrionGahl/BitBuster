using Godot;
using Logger = BitBuster.utils.Logger;

namespace BitBuster.weapon;

public partial class EffectPool : Area2D
{
	// TODO: Instantiate pool somewhere, have a set position method.
	// From there based on effect deal damage to player with damageTimer every so often, and apply given effect.
	// If entity needs to respond to effect they do it on their end such as slowing.
	// On Pool side check based on effect how it should respond to certain events. Such as fire bullet detonating oil/poisons.

	private Timer _despawnTimer;
	private Timer _damageTimer;

	private GpuParticles2D _poolEmitter;
	private GpuParticles2D _poolSubEmitter;
	
	
	public override void _Ready()
	{
		_despawnTimer = GetNode<Timer>("DespawnTimer");
		_damageTimer = GetNode<Timer>("DamageTimer");

		_poolEmitter = GetNode<GpuParticles2D>("PoolEmitter");
		_poolSubEmitter = GetNode<GpuParticles2D>("PoolSubEmitter");

		
		_despawnTimer.Timeout += OnDespawnTimeout;
		// _damageTimer.Timeout += OnDamageTimeout;
	}

	public override void _Process(double delta)
	{
		if (_damageTimer.TimeLeft > 0)
			return;
	
		Logger.Log.Information("POOL HIT");
	}

	private void OnDespawnTimeout()
	{
		QueueFree();
	}

	// private void OnDamageTimeout()
	// {
	// 	
	// }
}
