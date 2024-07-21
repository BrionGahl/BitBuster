using System.Collections.Generic;
using Godot;

namespace BitBuster.world;

public partial class Global : Node
{
	public List<PackedScene> CompleteItemPoolList { get; private set; }
	
	public int WorldLevel { get; set; } = 1;
	public int[,] MapGrid { get; set; }
	
	private string[] _itemNames;
	
	public override void _Ready()
	{
		CompleteItemPoolList = new List<PackedScene>();
		_itemNames = DirAccess.Open("res://scenes/subscenes/items/normal").GetFiles();

		for (int i = 0; i < _itemNames.Length; i++)
		{
			CompleteItemPoolList.Add(GD.Load<PackedScene>("res://scenes/subscenes/items/normal/" + _itemNames[i]));
		}
		
	}
}
