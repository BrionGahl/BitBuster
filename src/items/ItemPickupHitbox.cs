using BitBuster.component;
using BitBuster.entity;
using BitBuster.resource;
using BitBuster.world;
using Godot;

namespace BitBuster.items;

public partial class ItemPickupHitbox : Area2D
{

	private EntityStats _entityStats;
	
	[Export]
	public HealthComponent HealthComponent { get; set; }
	
	public Node2D ItemList { get; private set; }
	
	public override void _Ready()
	{
		ItemList = GetNode<Node2D>("ItemsList");
		_entityStats = GetParent<Entity>().EntityStats;
		
		BodyEntered += OnBodyEntered;
	}

	private void OnBodyEntered(Node body)
	{
		if (body is not Item)
			return;
		
		Item item = (Item)body;

		if (item.ItemType == ItemType.Normal)
		{
			_entityStats.AddItem(item);
			ItemList.CallDeferred("add_child", item.Duplicate());
		}
		
		if (item.AddedHealth > 0)
		{
			if (HealthComponent.CurrentHealth == HealthComponent.MaxHealth)
				return;
			HealthComponent.Heal(item.AddedHealth);
		}

		if (item.AddedOverheal > 0)
		{
			if (HealthComponent.Overheal == HealthComponent.MaxHealth)
				return;
			_entityStats.Overheal += item.AddedOverheal;
		}
		
		item.OnPickup();

		_entityStats.BombCount += item.AddedBombs;
		_entityStats.CreditCount += item.AddedCredit;
		_entityStats.KeyCardCount += item.AddedKeyCard;

		_entityStats.EmitStatChangeSignal();
	}
	
}
