using System.Collections.Generic;
using Godot;

namespace BitBuster.world;

public partial class Global : Node
{
	public Dictionary<int, PackedScene> CompleteItemPoolList { get; private set; }
	public Dictionary<int, PackedScene> CurrentRunItemPoolList { get; set; }
	
	public Dictionary<int, PackedScene> CompletePickupPoolList { get; private set; }
	
	public int WorldLevel { get; set; } = 1;
	public int[,] MapGrid { get; set; }
	
	private string[] _itemNames;
	private string[] _pickupNames;
	
	public override void _Ready()
	{
		CompleteItemPoolList = new Dictionary<int, PackedScene>();
		_itemNames = DirAccess.Open("res://scenes/subscenes/items/normal").GetFiles();
		
		CompletePickupPoolList = new Dictionary<int, PackedScene>();
		_pickupNames = DirAccess.Open("res://scenes/subscenes/items/pickup").GetFiles();
		
		for (int i = 0; i < _itemNames.Length; i++)
		{
			CompleteItemPoolList.Add(i, GD.Load<PackedScene>("res://scenes/subscenes/items/normal/" + _itemNames[i]));
		}

		for (int i = 0; i < _pickupNames.Length; i++)
		{
			CompletePickupPoolList.Add(i, GD.Load<PackedScene>("res://scenes/subscenes/items/pickup/" + _pickupNames[i]));
		}
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
