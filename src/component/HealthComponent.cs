using BitBuster.component;
using BitBuster.data;
using BitBuster.utils;
using Godot;

namespace BitBuster.Component;

public partial class HealthComponent : Node2D
{
	[Signal]
	public delegate void HealthChangeEventHandler();
	
	[Export]
	private StatsComponent _statsComponent;

	public float MaxHealth
	{
		get => _statsComponent.MaxHealth;
		set => _statsComponent.MaxHealth = value; 
	}
	
	public float CurrentHealth
	{
		get => _statsComponent.CurrentHealth;
		set => _statsComponent.CurrentHealth = value; 
	}

	private Timer _deathAnimationTimer;
	
	public override void _Ready()
	{
		_deathAnimationTimer = GetNode<Timer>("DeathAnimationTimer");
		
		CurrentHealth = MaxHealth;

		_deathAnimationTimer.Timeout += OnDeathAnimationTimeout;
	}

	public void LinkNodes()
	{
		_statsComponent = GetParent().GetNode<Node2D>("StatsComponent") as StatsComponent;
	}

	public void Damage(AttackData attackData)
	{
		Logger.Log.Information(this + " taking " + attackData.Damage + " damage.");
		
		CurrentHealth -= attackData.Damage;
		
		if (CurrentHealth <= 0)
		{
			CurrentHealth = 0;
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
