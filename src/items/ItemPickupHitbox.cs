using BitBuster.component;
using BitBuster.items.pickup;
using BitBuster.world;
using Godot;

namespace BitBuster.items;

public partial class ItemPickupHitbox : Area2D
{
	
	[Export]
	public StatsComponent StatsComponent { get; set; }
	
	[Export]
	public HealthComponent HealthComponent { get; set; }
	
	public Node2D ItemList { get; private set; }
	
	public override void _Ready()
	{
		ItemList = GetNode<Node2D>("ItemsList");
		
		AreaEntered += OnAreaEntered;
	}

	private void OnAreaEntered(Area2D area)
	{
		if (area is not Item)
			return;

		Item item = (Item)area;
		
		if (item.ItemType == ItemType.Normal)
		{
			StatsComponent.AddItem(item);
			ItemList.CallDeferred("add_child", item.Duplicate());
		}
		
		if (item.AddedHealth > 0)
		{
			if (HealthComponent.CurrentHealth == HealthComponent.MaxHealth)
				return;
			HealthComponent.Heal(item.AddedHealth);
		}

		StatsComponent.BombCount += item.AddedBombs;
		StatsComponent.CreditCount += item.AddedCredit;
		StatsComponent.KeyCardCount += item.AddedKeyCard;
		StatsComponent.Overheal += item.AddedOverheal;


		item.OnPickup();
	}
	
}
