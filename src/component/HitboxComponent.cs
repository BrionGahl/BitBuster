using BitBuster.data;
using Godot;

namespace BitBuster.Component;

public partial class HitboxComponent: Area2D
{

	[Export]
	private HealthComponent _healthComponent;
	
	public void LinkNodes()
	{
		_healthComponent = GetParent().GetNode<Node2D>("HealthComponent") as HealthComponent;
	}

	public void Damage(AttackData attackData)
	{
		if (_healthComponent != null)
		{
			_healthComponent.Damage(attackData);
		}
	}
}
