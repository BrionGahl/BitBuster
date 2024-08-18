using BitBuster.data;
using Godot;

namespace BitBuster.component;

public partial class HitboxComponent: Area2D
{

	private HealthComponent _healthComponent;

	[Export]
	public HealthComponent HealthComponent
	{
		get => _healthComponent;
		set => _healthComponent = value;
	}
	
	public void Damage(AttackData attackData)
	{
		if (HealthComponent != null)
		{
			HealthComponent.Damage(attackData);
		}
	}
}
