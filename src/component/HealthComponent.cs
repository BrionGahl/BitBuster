using BitBuster.data;
using Godot;

namespace BitBuster.Component;

public partial class HealthComponent : Node2D
{
	[Signal]
	public delegate void HealthChangeEventHandler();


	[Export]
	public float MaxHealth { get; set; } = 3.0f;
	
	[Export]
	public float CurrentHealth { get; set; }

	private Timer _deathAnimationTimer;
	
	public override void _Ready()
	{
		_deathAnimationTimer = GetNode<Timer>("DeathAnimationTimer");
		
		CurrentHealth = MaxHealth;

		_deathAnimationTimer.Timeout += OnDeathAnimationTimeout;
	}

	public void Damage(AttackData attackData)
	{
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
