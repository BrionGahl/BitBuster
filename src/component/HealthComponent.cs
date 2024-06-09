using BitBuster.component;
using BitBuster.data;
using BitBuster.utils;
using Godot;

namespace BitBuster.component;

public partial class HealthComponent : Node2D
{
	[Signal]
	public delegate void HealthChangeEventHandler();

	[Signal]
	public delegate void HealthIsZeroEventHandler();
	
	[Export]
	public StatsComponent StatsComponent { get; set; }

	private GpuParticles2D _particleDeath;

	public float MaxHealth
	{
		get => StatsComponent.MaxHealth;
		set => StatsComponent.MaxHealth = value; 
	}
	
	public float CurrentHealth
	{
		get => StatsComponent.CurrentHealth;
		set => StatsComponent.CurrentHealth = value; 
	}

	private Timer _iFrameTimer;
	private AnimationPlayer _parentAnimationPlayer;
	
	private bool _canBeHit = true;
	
	public override void _Ready()
	{
		CurrentHealth = MaxHealth;
		_iFrameTimer = GetNode<Timer>("IFrameTimer");
		_parentAnimationPlayer = GetParent().GetNodeOrNull<AnimationPlayer>("AnimationPlayer");

		_iFrameTimer.Timeout += IFrameTimeout;
	}

	public void Damage(AttackData attackData)
	{
		if (!_canBeHit)
			return;
		
		Logger.Log.Information(GetParent().Name + " taking " + attackData.Damage + " damage.");
		_canBeHit = false;

		if (_parentAnimationPlayer != null)
			_parentAnimationPlayer.Play("effect_damage_blink", -1D, StatsComponent.ITime);
		
		CurrentHealth -= attackData.Damage;
		
		if (CurrentHealth <= 0)
		{
			CurrentHealth = 0;
			EmitSignal(SignalName.HealthIsZero);
		}
	
		EmitSignal(SignalName.HealthChange);
		_iFrameTimer.Start(StatsComponent.ITime);
	}

	public void Damage(float damage)
	{
		if (!_canBeHit)
			return;
		
		Logger.Log.Information(this + " taking " + damage + " damage.");
		
		CurrentHealth -= damage;
		
		if (CurrentHealth <= 0)
		{
			CurrentHealth = 0;
			EmitSignal(SignalName.HealthIsZero);
		}
	
		EmitSignal(SignalName.HealthChange);
	}

	
	public void Heal(float heal)
	{
		CurrentHealth += heal;
		
		if (CurrentHealth > MaxHealth)
		{
			CurrentHealth = MaxHealth;
		}
	
		EmitSignal(SignalName.HealthChange);
	}

	private void IFrameTimeout()
	{
		_canBeHit = true;
	}
}
