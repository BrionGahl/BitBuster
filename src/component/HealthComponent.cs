using Godot;

namespace BitBuster.Component;

public partial class HealthComponent : Node2D
{

	[Export]
	public float MaxHealth { get; set; } = 10.0f;
	
	[Export]
	public float CurrentHealth { get; set; }

	public override void _Ready()
	{
		CurrentHealth = MaxHealth;
	}

	public void Damage(float damage)
	{
		CurrentHealth -= damage;
		
		if (CurrentHealth <= 0)
			GetParent().QueueFree();
	}
}
