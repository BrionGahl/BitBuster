using BitBuster.data;
using BitBuster.resource;
using BitBuster.utils;
using BitBuster.weapon;
using Godot;

namespace BitBuster.component;

public partial class HitboxComponent: Area2D
{

	private int[] _statusTicks;
	
	private GpuParticles2D _onStatusEmitter;
	private Timer _statusTimer;

	private EntityStats _entityStats;

	private bool _isOiled;
	private bool _isSludged;

	[Export]
	public HealthComponent HealthComponent { get; set; }


	public SourceType Source => HealthComponent.EntityStats.Source;

	public override void _Ready()
	{
		_onStatusEmitter = GetNode<GpuParticles2D>("OnStatusEmitter");
		_statusTimer = GetNode<Timer>("Timer");

		_statusTicks = new int[3];

		_statusTimer.Timeout += OnTimeout;
	}

	public void Damage(AttackData attackData)
	{
		HealthComponent?.Damage(attackData);
	}

	public void ApplyStatus(EffectType effects)
	{
		
		if (effects.HasFlag(EffectType.Fire))
		{
			_onStatusEmitter.Modulate = Colors.Orange;
			_statusTicks[0] = 15;
		}
		if (effects.HasFlag(EffectType.Oil) && !effects.HasFlag(EffectType.Sludge))
		{
			_onStatusEmitter.Modulate = Colors.SaddleBrown;
			_statusTicks[1] = 30;
		}
		if (effects.HasFlag(EffectType.Sludge))
		{
			_onStatusEmitter.Modulate = Colors.Purple;
			_statusTicks[2] = 30;
		}
		_onStatusEmitter.Emitting = true;
		_statusTimer.Start();
	}
	
	private void OnTimeout()
	{

		if (_statusTicks[0] > 0) // Fire
		{
			HealthComponent.Damage(0.11f);
			_statusTicks[0]--;
		}
		if (_statusTicks[1] > 0) // Oil
		{
			if (!_isOiled)
				HealthComponent.EntityStats.Speed *= 1.25f;
			_isOiled = true;
			_statusTicks[1]--;
		}
		if (_statusTicks[2] > 0) // Sludge
		{
			if (!_isSludged)
				HealthComponent.EntityStats.Speed /= 2;
			_isSludged = true;
			_statusTicks[2]--;
		}
		
		if (_statusTicks[0] > 0 || _statusTicks[1] > 0 || _statusTicks[2] > 0)
			return;
		
		Logger.Log.Information("HERE");
		
		_statusTimer.Stop();
		_onStatusEmitter.Emitting = false;
		
		if (_isOiled)
		{
			HealthComponent.EntityStats.Speed /= 1.25f;
			_isOiled = false;
		}

		if (_isSludged)
		{
			HealthComponent.EntityStats.Speed *= 2f;
			_isSludged = false;
		}
	}
}
