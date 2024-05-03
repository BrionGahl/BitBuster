using Godot;

namespace BitBuster.Component;

public partial class HealthComponent : Node2D
{

	private float _health;

	[Export] public float MaxHealth { get; set; } = 10.0f;


	public override void _Ready()
	{
		_health = MaxHealth;
	}

	public void Damage(float damage)
	{
		_health -= damage;
		
		if (_health <= 0)
			GetParent().QueueFree();
	}
	
}
