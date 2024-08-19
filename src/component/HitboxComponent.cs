using BitBuster.data;
using BitBuster.weapon;
using Godot;

namespace BitBuster.component;

public partial class HitboxComponent: Area2D
{
	[Export]
	public HealthComponent HealthComponent { get; set; }

	public SourceType Source => HealthComponent.EntityStats.Source;

	public void Damage(AttackData attackData)
	{
		if (HealthComponent != null)
		{
			HealthComponent.Damage(attackData);
		}
	}
}
