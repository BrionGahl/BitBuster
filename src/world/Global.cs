using System.Collections.Generic;
using BitBuster.items;
using Godot;

namespace BitBuster.world;

public partial class Global : Node
{
	public PackedScene GamePackedScene { get; private set; }
	public PackedScene MainMenuPackedScene { get; private set; }
	
	public Dictionary<int, PackedScene> CompleteItemPoolList { get; private set; }
	public Dictionary<int, PackedScene> CurrentRunItemPoolList { get; set; }
	
	public Dictionary<int, PackedScene> CompletePickupPoolList { get; private set; }
	
	public int WorldLevel { get; set; } = 1;
	public int[,] MapGrid { get; set; }
	
	private string[] _itemNames;
	private string[] _pickupNames;
	
	public override void _Ready()
	{
		GamePackedScene = ResourceLoader.Load<PackedScene>("res://scenes/subscenes/procedural/world.tscn");
		MainMenuPackedScene = ResourceLoader.Load<PackedScene>("res://scenes/subscenes/menu/root_menu.tscn");
		
		CompleteItemPoolList = ItemScenes.GetCompleteNormalItemDictionary();

		CompletePickupPoolList = ItemScenes.GetCompletePickupItemDictionary();
	}

	public void PrepareCurrentRunItemPool()
	{
		CurrentRunItemPoolList = new Dictionary<int, PackedScene>(CompleteItemPoolList);
	}

	public void RemoveFromCurrentItemPool(int id)
	{
		CurrentRunItemPoolList.Remove(id);
	}
}
