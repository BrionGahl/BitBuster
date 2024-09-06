using BitBuster.data;
using BitBuster.resource;
using BitBuster.utils;
using BitBuster.weapon;
using Godot;

namespace BitBuster.component;

public partial class HealthComponent : Node2D
{
	[Signal]
	public delegate void HealthChangeEventHandler(float value);

	[Signal]
	public delegate void HealthIsZeroEventHandler();

	private EntityStats _entityStats;
	
	public EntityStats EntityStats
	{
		get => _entityStats;
		set
		{
			_entityStats = value;
			CurrentHealth = MaxHealth;
		}
	}
	
	public float MaxHealth
	{
		get => EntityStats.MaxHealth;
		private set => EntityStats.MaxHealth = value; 
	}
	
	public float CurrentHealth
	{
		get => EntityStats.CurrentHealth;
		private set => EntityStats.CurrentHealth = value; 
	}
	
	public float Overheal
	{
		get => EntityStats.Overheal;
		private set => EntityStats.Overheal = value; 
	}
	
	public EffectType StatusCondition { get; private set; }
	
	private Timer _iFrameTimer;
	private OverhealBurstComponent _overhealBurstComponent;
	
	private bool CanBeHit => _iFrameTimer.TimeLeft <= 0;

	
	public override void _Ready()
	{
		_iFrameTimer = GetNode<Timer>("IFrameTimer");
		_overhealBurstComponent = GetNode<OverhealBurstComponent>("OverhealBurstComponent");
	}

	public void Damage(AttackData attackData)
	{
		if (!CanBeHit)
			return;
		
		Logger.Log.Information(EntityStats.Name + " taking " + attackData.Damage + " damage.");
		
		if (Overheal > 0)
		{
			Overheal -= attackData.Damage;
			Overheal = Mathf.Floor(Overheal);
			if (Overheal < 0)
				Overheal = 0;
			if (EntityStats.OverhealBurst)
				_overhealBurstComponent.Burst(75f);
		}
		else 
			CurrentHealth -= attackData.Damage;
		
		if (CurrentHealth <= 0)
		{
			CurrentHealth = 0;
			EmitSignal(SignalName.HealthIsZero);
		}
	
		EmitSignal(SignalName.HealthChange, -attackData.Damage);
		_iFrameTimer.Start(EntityStats.ITime);
	}

	public void Damage(float damage)
	{
		if (!CanBeHit)
			return;
		
		Logger.Log.Information(EntityStats.Name + " taking " + damage + " damage.");

		if (Overheal > 0)
		{
			Overheal -= damage;
			Overheal = Mathf.Floor(Overheal);
			if (Overheal < 0)
				Overheal = 0;
			if (EntityStats.OverhealBurst)
				_overhealBurstComponent.Burst(75f);
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
}
