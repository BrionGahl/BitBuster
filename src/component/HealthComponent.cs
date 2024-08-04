using BitBuster.component;
using BitBuster.data;
using BitBuster.utils;
using Godot;

namespace BitBuster.component;

public partial class HealthComponent : Node2D
{
	[Signal]
	public delegate void HealthChangeEventHandler(float value);

	[Signal]
	public delegate void HealthIsZeroEventHandler();
	
	[Export]
	public StatsComponent StatsComponent { get; set; }
	
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
	
	public float Overheal
	{
		get => StatsComponent.Overheal;
		set => StatsComponent.Overheal = value; 
	}

	private Timer _iFrameTimer;
	private OverhealBurstComponent _overhealBurstComponent;
	
	private bool _canBeHit = true;
	
	public override void _Ready()
	{
		CurrentHealth = MaxHealth;
		_iFrameTimer = GetNode<Timer>("IFrameTimer");
		_overhealBurstComponent = GetNode<OverhealBurstComponent>("OverhealBurstComponent");
		
		_iFrameTimer.Timeout += OnIFrameTimeout;
	}

	public void Damage(AttackData attackData)
	{
		if (!_canBeHit)
			return;
		
		Logger.Log.Information(GetParent().Name + " taking " + attackData.Damage + " damage.");
		_canBeHit = false;

		if (Overheal > 0)
		{
			Overheal -= attackData.Damage;
			if (Overheal < 0)
				Overheal = 0;
			if (StatsComponent.OverhealBurst)
			{
				_overhealBurstComponent.Burst(75f);
			}
		}
		else 
			CurrentHealth -= attackData.Damage;
		
		if (CurrentHealth <= 0)
		{
			CurrentHealth = 0;
			EmitSignal(SignalName.HealthIsZero);
		}
	
		EmitSignal(SignalName.HealthChange, -attackData.Damage);
		_iFrameTimer.Start(StatsComponent.ITime);
	}

	public void Damage(float damage)
	{
		if (!_canBeHit)
			return;
		
		Logger.Log.Information(GetParent().Name + " taking " + damage + " damage.");

		if (Overheal > 0)
		{
			Overheal -= damage;
			if (Overheal < 0)
				Overheal = 0;
		}
		else 
			CurrentHealth -= damage;

		
		if (CurrentHealth <= 0)
		{
			CurrentHealth = 0;
			EmitSignal(SignalName.HealthIsZero);
			return;
		}
	
		EmitSignal(SignalName.HealthChange, -damage);
	}

	
	public void Heal(float heal)
	{
		CurrentHealth += heal;
		
		if (CurrentHealth > MaxHealth)
		{
			CurrentHealth = MaxHealth;
		}
	
		EmitSignal(SignalName.HealthChange, heal);
	}

	private void OnIFrameTimeout()
	{
		_canBeHit = true;
	}
}
