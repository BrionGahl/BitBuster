using System;
using System.Collections.Generic;
using System.Drawing;
using BitBuster.data;
using BitBuster.entity.player;
using Godot;
using Serilog.Core;
using Logger = BitBuster.utils.Logger;

namespace BitBuster.procedural;

public partial class Floor : Node2D
{
	private int _roomCount;
	private int _roomLimit;
	private Queue<Vector2I> _roomQueue;
	private List<Vector2I> _endRooms;

	private RandomNumberGenerator _random;
	private Rooms _rooms;

	public int Level { get; set; } = 1;
	public int[,] MapGrid { get; set; }
	
	public CharacterBody2D Player { get; set; }
	
	public PackedScene Rooms { get; set; }
	public SceneTree SceneTree { get; set; }
	public TileMap LevelMain { get; set; }
	public Node2D LevelExtra { get; set; }
	
	
	public override void _Ready()
	{
		Rooms = GD.Load<PackedScene>("res://scenes/subscenes/procedural/rooms.tscn");
		_rooms = Rooms.Instantiate<Node2D>() as Rooms;
		
		SceneTree = GetTree();
		LevelMain = GetNode<TileMap>("Level/TileMapMain");
		LevelExtra = GetNode<Node2D>("Level/Extra");
		
		_random = new RandomNumberGenerator();
		_random.Randomize();
		
		_roomLimit = _random.RandiRange(0,2) + 5 + Level * 2;

		GenerateLevel();
	}


	private void GenerateLevel()
	{
		GenerateMap();
		PrintMap(new Vector2I(4, 3));
		PlaceRooms();
	}


	private void GenerateMap()
	{
		_roomQueue = new Queue<Vector2I>();
		_endRooms = new List<Vector2I>();

		MapGrid = new int[9, 8];

		MapGrid[4, 3] = (int)RoomType.START;
		
		_roomCount = 1;
		_roomQueue.Enqueue(new Vector2I(4, 3));
		
		List<Vector2I> neighbors = new List<Vector2I>();

		while (_roomQueue.Count > 0)
		{
			Logger.Log.Information("Generating Room: " + _roomCount + " / " + _roomLimit);
			Vector2I currRoom = _roomQueue.Dequeue();
			
			int addedNeighbors = 0;
			neighbors.Add(new Vector2I(currRoom.X, currRoom.Y + 1)); // above
			neighbors.Add(new Vector2I(currRoom.X, currRoom.Y - 1)); // below
			neighbors.Add(new Vector2I(currRoom.X + 1, currRoom.Y)); // right
			neighbors.Add(new Vector2I(currRoom.X - 1, currRoom.Y)); // left

			for (int i = 0; i < neighbors.Count; i++)
			{
				if (neighbors[i].X == 0 || neighbors[i].X == 8)
				{
					neighbors.RemoveAt(i);
				} else if (neighbors[i].Y < 0 || neighbors[i].Y > 7)
				{
					neighbors.RemoveAt(i);
				}
			}
			
			for (int i = 0; i < neighbors.Count; i++)
			{
				if (_roomCount == _roomLimit) // if we are already at room cap stop.
				{
					Logger.Log.Debug("FloorManager - New room failed due to room limit.");
					continue; // GIVE UP
				}
				if (MapGrid[neighbors[i].X, neighbors[i].Y] != 0) // if there is already a room there stop.
				{
					Logger.Log.Debug("FloorManager - New room failed due to room already existing.");
					continue; // GIVE UP
				}
				if (Convert.ToBoolean(_random.RandiRange(0, 1))) // 50% chance to randomly stop.
				{
					Logger.Log.Debug("FloorManager - New room failed due to 50/50 change.");
					continue; // GIVE UP
				}

				int filledNeighbors = 0;
				List<Vector2I> neighborsOfNeighbor = new List<Vector2I>();
				neighborsOfNeighbor.Add(new Vector2I(neighbors[i].X, neighbors[i].Y + 1));
				neighborsOfNeighbor.Add(new Vector2I(neighbors[i].X, neighbors[i].Y - 1));
				neighborsOfNeighbor.Add(new Vector2I(neighbors[i].X + 1, neighbors[i].Y));
				neighborsOfNeighbor.Add(new Vector2I(neighbors[i].X - 1, neighbors[i].Y));
				
				for (int j = 0; j < neighborsOfNeighbor.Count; j++)
				{
					if (neighborsOfNeighbor[j].X == 0 || neighborsOfNeighbor[j].X == 8)
					{
						neighborsOfNeighbor.RemoveAt(j);
					} else if (neighborsOfNeighbor[j].Y < 0 || neighborsOfNeighbor[j].Y > 7)
					{
						neighborsOfNeighbor.RemoveAt(j);
					}
				}
				
				for (int j = 0; j < neighborsOfNeighbor.Count; j++)
				{
					if (MapGrid[neighborsOfNeighbor[j].X, neighborsOfNeighbor[j].Y] != 0)
						filledNeighbors++;
				}

				if (filledNeighbors > 1) // if the neighbor already has more than one filled neighbor then stop.
				{
					Logger.Log.Debug("FloorManager - New room failed due to neighbor of neighbor already existing.");
					continue; // GIVE UP
				}
				
				addedNeighbors += 1;
				MapGrid[neighbors[i].X, neighbors[i].Y] = (int)RoomType.NORMAL;
				_roomCount += 1;
				_roomQueue.Enqueue(neighbors[i]);
			}

			if (addedNeighbors == 0)
			{
				Logger.Log.Debug("FloorManager - End Room found at: " + currRoom + ".");
				_endRooms.Add(currRoom);
			}
		}
		
		if (_endRooms.Count < 3)
		{
			Logger.Log.Information("FloorManager - Not enough end rooms. Regenerating...");
			GenerateMap();
		}

		_endRooms.Reverse();
		MapGrid[_endRooms[0].X, _endRooms[2].Y] = (int)RoomType.BOSS;
		MapGrid[_endRooms[1].X, _endRooms[1].Y] = (int)RoomType.SECRET;
		MapGrid[_endRooms[2].X, _endRooms[0].Y] = (int)RoomType.TREASURE;
	}

	private void PlaceRooms()
	{
		for (int x = 0; x < 9; x++)
		{
			for (int y = 0; y < 8; y++)
			{
				if (MapGrid[x, y] == 0)
					continue;
				
				CopyRoom(new Vector2I(x, y), (RoomType)MapGrid[x, y]);
			}
		}
	}


	private void CopyRoom(Vector2I offset, RoomType type)
	{
		Vector2I worldOffset = GridToWorld(offset);
		Vector2I mapOffset = GridToMap(offset);
		RoomData data = _rooms.GetRoomData(type);

		for (int i = 0; i < data.Objects.Count; i++)
		{
			Logger.Log.Debug("Adding Child to Extra...");
			Node2D newObject = data.Objects[i].Duplicate() as Node2D;
			newObject.Position += worldOffset;
			LevelExtra.AddChild(newObject);

			if (newObject.IsInGroup("player"))
				Player = newObject as Player;
		}

		for (int i = 0; i < data.TileMap.Count; i++)
		{
			LevelMain.SetCell(0, mapOffset + data.TileMap[i].Offset, data.TileMap[i].TargetId, data.TileMap[i].AtlasCoords);
		}
	}
	
	private Vector2I GridToMap(Vector2I vector)
	{
		return _rooms.RoomSize * vector;
	}

	private Vector2I GridToWorld(Vector2I vector)
	{
		return _rooms.CellSize * _rooms.RoomSize * vector;
	}
	
	public void PrintMap(Vector2I currentPosition)
	{
		for (int i = 0; i < 8; i++)
		{
			Console.Write("\t");
			for (int j = 0; j < 9; j++)
			{
				if (j == currentPosition.X && i == currentPosition.Y)
					Console.Write(MapGrid[j, i] + " ");
				else 
					Console.Write(MapGrid[j, i] + " ");
			}
			Console.WriteLine("");
		}
	}
}
