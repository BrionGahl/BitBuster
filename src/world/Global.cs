using System.Collections.Generic;
using Godot;

namespace BitBuster.world;

public partial class Global : Node
{
	public List<PackedScene> CompleteItemPoolList { get; private set; }
	public List<PackedScene> CurrentRunItemPoolList { get; set; }
	
	public List<PackedScene> CompletePickupPoolList { get; private set; }
	
	public int WorldLevel { get; set; } = 1;
	public int[,] MapGrid { get; set; }
	
	private string[] _itemNames;
	private string[] _pickupNames;
	
	public override void _Ready()
	{
		CompleteItemPoolList = new List<PackedScene>();
		_itemNames = DirAccess.Open("res://scenes/subscenes/items/normal").GetFiles();
		
		CompletePickupPoolList = new List<PackedScene>();
		_pickupNames = DirAccess.Open("res://scenes/subscenes/items/pickup").GetFiles();
		
		for (int i = 0; i < _itemNames.Length; i++)
		{
			CompleteItemPoolList.Add(GD.Load<PackedScene>("res://scenes/subscenes/items/normal/" + _itemNames[i]));
		}

		for (int i = 0; i < _pickupNames.Length; i++)
		{
			CompletePickupPoolList.Add(GD.Load<PackedScene>("res://scenes/subscenes/items/pickup/" + _pickupNames[i]));
		}
	}

	public void PrepareCurrentRunItemPool()
	{
		CurrentRunItemPoolList = new List<PackedScene>(CompleteItemPoolList);
	}
}
