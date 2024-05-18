using BitBuster.data;
using Godot;

namespace BitBuster.component;

public partial class HitboxComponent: Area2D
{

	[Export]
	public HealthComponent HealthComponent { get; set; }

	public void Damage(AttackData attackData)
	{
		if (HealthComponent != null)
		{
			HealthComponent.Damage(attackData);
		}
	}
}
