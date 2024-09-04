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
	private bool _isShocked;

	[Export]
	public HealthComponent HealthComponent { get; set; }


	public SourceType Source => HealthComponent.EntityStats.Source;

	public override void _Ready()
	{
		_onStatusEmitter = GetNode<GpuParticles2D>("OnStatusEmitter");
		_statusTimer = GetNode<Timer>("Timer");

		_statusTicks = new int[5];

		_statusTimer.Timeout += OnTimeout;
	}

	public void Damage(AttackData attackData)
	{
		HealthComponent?.Damage(attackData);
		
		ApplyStatus(attackData.Effects);
	}

	public void ApplyStatus(EffectType effects)
	{
		
		if (effects.HasFlag(EffectType.Fire))
		{
			_onStatusEmitter.Modulate = Colors.Orange;
			_statusTicks[0] = 15;
		}
		if (effects.HasFlag(EffectType.Water) && !effects.HasFlag(EffectType.Smoke))
		{
			_onStatusEmitter.Modulate = Colors.Blue;
			_statusTicks[0] = 0; // put out the fire.
			_statusTicks[4] = 30;
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
		if (effects.HasFlag(EffectType.Shocked))
		{
			_onStatusEmitter.Modulate = Colors.Yellow;
			_statusTicks[3] = 5 + _statusTicks[4];
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
		if (_statusTicks[3] > 0) // Sludge
		{
			if (!_isShocked)
				HealthComponent.EntityStats.Speed /= 100 ;
			_isShocked = true;
			_statusTicks[3]--;
		}
		
		if (_statusTicks[0] > 0 || _statusTicks[1] > 0 || _statusTicks[2] > 0 || _statusTicks[3] > 0)
			return;
		
		
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
		
		if (_isShocked)
		{
			HealthComponent.EntityStats.Speed *= 100f;
			_isShocked = false;
		}
	}
}
