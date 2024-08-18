using BitBuster.component;
using BitBuster.resource;
using BitBuster.world;
using Godot;

namespace BitBuster.item;

public partial class ItemPickupHitbox : Area2D
{
	private Global _global;
	
	[Export]
	public HealthComponent HealthComponent { get; set; }
	public EntityStats EntityStats { get; set; }
	public Node2D ItemList { get; private set; }
	
	public override void _Ready()
	{
		ItemList = GetNode<Node2D>("ItemsList");
		_global = GetNode<Global>("/root/Global");
		
		BodyEntered += OnBodyEntered;
	}

	private void OnBodyEntered(Node body)
	{
		if (body is not BitBuster.item.Item)
			return;
		
		item.Item item = (item.Item)body;

		if (EntityStats.CreditCount < item.CreditCost)
			return;

		EntityStats.CreditCount -= item.CreditCost;
		item.SetCollisionLayerValue((int)BBCollisionLayer.Item, false);

		if (item.ItemType == ItemType.Normal)
		{
			EntityStats.AddItem(item);
			ItemList.CallDeferred("add_child", item.Duplicate());
			_global.RemoveFromCurrentItemPool(item.ItemId);
		}
		
		if (item.AddedHealth > 0)
		{
			if (HealthComponent.CurrentHealth >= HealthComponent.MaxHealth)
				return;
			HealthComponent.Heal(item.AddedHealth);
		}

		if (item.AddedOverheal > 0)
		{
			if (HealthComponent.Overheal >= HealthComponent.MaxHealth)
				return;
			EntityStats.Overheal += item.AddedOverheal;
			HealthComponent.Heal(0);
		}
		
		item.OnPickup();

		EntityStats.BombCount += item.AddedBombs;
		EntityStats.CreditCount += item.AddedCredit;
		EntityStats.KeyCardCount += item.AddedKeyCard;

		EntityStats.EmitStatChangeSignal();
	}
	
}
