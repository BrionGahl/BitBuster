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

	private Timer _deathAnimationTimer;
	
	public override void _Ready()
	{
		CurrentHealth = MaxHealth;
	}

	public void Damage(AttackData attackData)
	{
		Logger.Log.Information(GetParent().Name + " taking " + attackData.Damage + " damage.");
		
		CurrentHealth -= attackData.Damage;
		
		if (CurrentHealth <= 0)
		{
			CurrentHealth = 0;
			EmitSignal(SignalName.HealthIsZero);
		}
	
		EmitSignal(SignalName.HealthChange);
	}

	public void Damage(float damage)
	{
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

	private void OnDeathAnimationTimeout()
	{
		GetParent().QueueFree();
	}
}
