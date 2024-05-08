using BitBuster.component;
using BitBuster.data;
using BitBuster.utils;
using Godot;

namespace BitBuster.component;

public partial class HealthComponent : Node2D
{
	[Signal]
	public delegate void HealthChangeEventHandler();
	
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
		_deathAnimationTimer = GetNode<Timer>("DeathAnimationTimer");
		_particleDeath = GetNode<GpuParticles2D>("ParticleDeath");
		
		CurrentHealth = MaxHealth;

		_deathAnimationTimer.Timeout += OnDeathAnimationTimeout;
	}

	public void Damage(AttackData attackData)
	{
		Logger.Log.Information(this + " taking " + attackData.Damage + " damage.");
		
		CurrentHealth -= attackData.Damage;
		
		if (CurrentHealth <= 0)
		{
			CurrentHealth = 0;
			_particleDeath.Emitting = true;
			_deathAnimationTimer.Start();
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
