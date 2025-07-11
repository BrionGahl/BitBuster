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

	private AudioStreamPlayer2D _normalSound;
	private AudioStreamPlayer2D _pickupSound;
	
	public override void _Ready()
	{
		ItemList = GetNode<Node2D>("ItemsList");
		_global = GetNode<Global>("/root/Global");

		_normalSound = GetNode<AudioStreamPlayer2D>("NormalSound");
		_pickupSound = GetNode<AudioStreamPlayer2D>("PickupSound");
		
		BodyEntered += OnBodyEntered;
	}

	private void OnBodyEntered(Node body)
	{
		if (body is not Item)
			return;
		
		Item item = (Item)body;

		if (EntityStats.CreditCount < item.CreditCost)
			return;

		EntityStats.CreditCount -= item.CreditCost;

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
		item.SetCollisionLayerValue((int)BbCollisionLayer.Item, false);

		
		item.OnPickup();

		EntityStats.BombCount += item.AddedBombs;
		EntityStats.CreditCount += item.AddedCredit;
		EntityStats.KeyCardCount += item.AddedKeyCard;
		
		if (item.ItemType == ItemType.Normal)
			_normalSound.Play();
		else
			_pickupSound.Play();
		
		EntityStats.EmitStatChangeSignal();
	}
	
}
