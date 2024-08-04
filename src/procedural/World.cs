using System;
using System.Collections.Generic;
using BitBuster.data;
using BitBuster.entity.enemy;
using BitBuster.entity.player;
using BitBuster.items;
using BitBuster.tiles;
using BitBuster.utils;
using BitBuster.world;
using Godot;

namespace BitBuster.procedural;

public partial class World : Node2D
{
	private int _roomCount;
	private int _roomLimit;
	private Queue<Vector2I> _roomQueue;
	private List<Vector2I> _endRooms;

	private RandomNumberGenerator _random;
	private Rooms _rooms;

	private int[,] _mapGrid;

	private PackedScene _roomsScene;
	private PackedScene _doorScene;

	private Global _global;
	private GlobalEvents _globalEvents;
	
	private List<PackedScene> _availableLootPool;
	
	private NavigationRegion2D _levelRegion;
	private TileMap _levelMain;
	private TileMap _levelSemi;
	
	private Node2D _levelBakeable;
	private Node2D _levelExtra;
	
	private Player _levelPlayer;
	
	public override void _Ready()
	{
		_global = GetNode<Global>("/root/Global");
		_globalEvents = GetNode<GlobalEvents>("/root/GlobalEvents");

		_roomsScene = GD.Load<PackedScene>("res://scenes/subscenes/procedural/rooms.tscn");
		_doorScene = GD.Load<PackedScene>("res://scenes/subscenes/tiles/door.tscn");

		_levelRegion = GetNode<NavigationRegion2D>("Level/NavRegion");
		_levelMain = GetNode<TileMap>("Level/NavRegion/TileMapMain");
		_levelSemi = GetNode<TileMap>("Level/NavRegion/TileMapSemi");
		_levelBakeable = GetNode<Node2D>("Level/NavRegion/Bakeable");
		_levelExtra = GetNode<Node2D>("Level/NavRegion/Extra");
		
		_levelPlayer = GetNode<CharacterBody2D>("Level/Player") as Player;
		
		_availableLootPool = new List<PackedScene>(_global.CompleteItemPoolList);
		_random = new RandomNumberGenerator();
		
		_globalEvents.IncrementAndGenerateLevel += OnIncrementAndGenerateLevel;
		_globalEvents.BakeNavigationMesh += OnBakeNavigationMesh;
		
		GenerateLevel();
	}


	private void GenerateLevel()
	{
		_roomLimit = _random.RandiRange(0, 2) + 5 + _global.WorldLevel * 2;
		_rooms = _roomsScene.Instantiate<Node2D>() as Rooms;
		_random.Randomize();
		
		GenerateMap();
		PrintMap(new Vector2I(4, 3));
		PlaceRooms();
		
		_rooms.QueueFree();
	}

	private void CleanLevel()
	{
		_levelMain.Clear();
		_levelSemi.Clear();
		
		foreach(Node child in _levelBakeable.GetChildren())
			child.QueueFree();
		
		foreach (Node child in _levelExtra.GetChildren())
			child.QueueFree();
	}

	private void OnIncrementAndGenerateLevel()
	{
		CleanLevel();
		
		_global.WorldLevel++;
		GenerateLevel();
	}

	private void OnBakeNavigationMesh()
	{
		Logger.Log.Information("Rebakeing...");
		_levelRegion.BakeNavigationPolygon();
	}

	private void GenerateMap()
	{
		Logger.Log.Information("Generating level: {@worldLevel}", _global.WorldLevel);
		
		_roomQueue = new Queue<Vector2I>();
		_endRooms = new List<Vector2I>();

		_global.MapGrid = new int[9, 8];
		_mapGrid = _global.MapGrid;

		_mapGrid[4, 3] = (int)RoomType.START;

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

			// Out of bounds check
			for (int i = 0; i < neighbors.Count; i++)
			{
				if (neighbors[i].X == 0 || neighbors[i].X == 8)
				{
					neighbors.RemoveAt(i);
				}
				else if (neighbors[i].Y < 0 || neighbors[i].Y > 7)
				{
					neighbors.RemoveAt(i);
				}
			}

			// Extra Room Condition Checks
			for (int i = 0; i < neighbors.Count; i++)
			{
				if (_roomCount == _roomLimit) // if we are already at room cap stop.
				{
					Logger.Log.Debug("FloorManager - New room failed due to room limit.");
					continue; // GIVE UP
				}

				if (_mapGrid[neighbors[i].X, neighbors[i].Y] != 0) // if there is already a room there stop.
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

				// Out of bounds check
				for (int j = 0; j < neighborsOfNeighbor.Count; j++)
				{
					if (neighborsOfNeighbor[j].X == 0 || neighborsOfNeighbor[j].X == 8)
					{
						neighborsOfNeighbor.RemoveAt(j);
					}
					else if (neighborsOfNeighbor[j].Y < 0 || neighborsOfNeighbor[j].Y > 7)
					{
						neighborsOfNeighbor.RemoveAt(j);
					}
				}

				for (int j = 0; j < neighborsOfNeighbor.Count; j++)
				{
					if (_mapGrid[neighborsOfNeighbor[j].X, neighborsOfNeighbor[j].Y] != 0)
						filledNeighbors++;
				}

				if (filledNeighbors > 1) // if the neighbor already has more than one filled neighbor then stop.
				{
					Logger.Log.Debug("FloorManager - New room failed due to neighbor of neighbor already existing.");
					continue; // GIVE UP
				}

				Logger.Log.Information("Adding a room at: " + neighbors[i].X + ", " + neighbors[i].Y);
				addedNeighbors += 1;
				_mapGrid[neighbors[i].X, neighbors[i].Y] = (int)RoomType.NORMAL;
				_roomCount += 1;
				_roomQueue.Enqueue(neighbors[i]);
			}

			Logger.Log.Information("Added neigbors for (" + currRoom.X + "," + currRoom.Y + ") room: " +
								   addedNeighbors);
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

		
		// Add end room stuff
		_endRooms.Reverse();
		_mapGrid[_endRooms[0].X, _endRooms[0].Y] = (int)RoomType.BOSS;
		_mapGrid[_endRooms[1].X, _endRooms[1].Y] = (int)RoomType.STORE;
		_mapGrid[_endRooms[2].X, _endRooms[2].Y] = (int)RoomType.TREASURE;
		
		
		// Quick scan through to see if we can make smaller rooms.
		for (int y = 0; y < 8; y++)
		{
			for (int x = 0; x < 9; x++)
			{
				float chance = _random.Randf();
				if (CheckLrRoom(new Vector2I(x, y)) && chance > 0.65)
					_mapGrid[x, y] = (int)RoomType.LR_NORMAL;
				if (CheckTbRoom(new Vector2I(x, y)) && chance > 0.65)
					_mapGrid[x, y] = (int)RoomType.TB_NORMAL;
			}
		}
	}

	private void PlaceRooms()
	{
		NavigationPolygon poly = new NavigationPolygon();
		poly.AgentRadius = 4;
		for (int x = 0; x < 9; x++)
		{
			for (int y = 0; y < 8; y++)
			{
				if (_mapGrid[x, y] == 0)
					continue;

				List<Vector2I> adjacentRooms = new List<Vector2I>();
				
				if (y - 1 >= 0 && _mapGrid[x, y - 1] != 0)
					adjacentRooms.Add(Vector2I.Up);
				if (y + 1 < 8 && _mapGrid[x, y + 1] != 0)
					adjacentRooms.Add(Vector2I.Down);
				if (x - 1 >= 0 && _mapGrid[x - 1, y] != 0)
					adjacentRooms.Add(Vector2I.Left);
				if (x + 1 < 9 && _mapGrid[x + 1, y] != 0)
					adjacentRooms.Add(Vector2I.Right);

				CopyRoom(new Vector2I(x, y), (RoomType)_mapGrid[x, y], adjacentRooms);
				poly.AddOutline(new Vector2[] {GridToWorld(new Vector2I(x, y)), GridToWorld(new Vector2I(x, y) + new Vector2I(320, 0)), GridToWorld(new Vector2I(x, y)) + new Vector2I(320, 320), GridToWorld(new Vector2I(x, y) + new Vector2I(0, 320))});
			}
		}
		_levelRegion.NavigationPolygon = poly;
		_levelRegion.BakeNavigationPolygon();
	}

	private void CopyRoom(Vector2I offset, RoomType type, List<Vector2I> adjacentRooms)
	{
		Vector2I worldOffset = GridToWorld(offset);
		Vector2I mapOffset = GridToMap(offset);
		
		RoomData data = _rooms.GetRoomData(type);
		
		switch (type)
		{
			case (RoomType.TREASURE):
				int itemToSpawn = _random.RandiRange(0, _availableLootPool.Count - 1);
				Item item = _availableLootPool[itemToSpawn].Instantiate<Area2D>() as Item; // TODO: Error on levels post item count...
				_availableLootPool.RemoveAt(itemToSpawn);
			
				item.Position += worldOffset + new Vector2I(160, 160);
				_levelExtra.AddChild(item);
				break;
		}
		
		for (int i = 0; i < data.Objects.Count; i++)
		{
			Logger.Log.Debug("Adding Child to Extra...");
			Node2D newObject = data.Objects[i].Duplicate() as Node2D;
			
			newObject.Position += worldOffset;
			
			if (newObject.IsInGroup(Groups.GroupPlayer))
			{
				_levelPlayer.Position = newObject.Position;
				Logger.Log.Information(newObject.Position.ToString());
				newObject.QueueFree();
				continue;
			}
			
			if (newObject.IsInGroup(Groups.GroupEnemy))
				(newObject as Enemy).SpawnPosition = newObject.Position;
			
			_levelExtra.AddChild(newObject);
			
			if (newObject.IsInGroup(Groups.GroupEnemy))
			{
				float chance = 1 - Mathf.Log(_global.WorldLevel * 1.15f);
				if (_random.Randf() > chance)
					(newObject as Enemy).MakeElite((EliteType)_random.RandiRange(0, 3));
			}
		}
		
		for (int i = 0; i < data.TileMap.Count; i++)
		{
			// If a tile has a direction vector tied to it, it must be a type of door.
			if (adjacentRooms.Contains(data.TileMap[i].Direction))
			{
				Door door = _doorScene.Instantiate<Area2D>() as Door;
				door.SetDoorInfo(((Vector2)data.TileMap[i].Direction).Angle(), data.TileMap[i].Offset * _rooms.CellSize + worldOffset, data.TileMap[i].Direction * 32);
				_levelBakeable.AddChild(door);
				continue; 
			}
			
			if (data.TileMap[i].AtlasCoords.Equals(Constants.PitTile))
				_levelSemi.SetCell(0, mapOffset + data.TileMap[i].Offset, data.TileMap[i].TargetId, data.TileMap[i].AtlasCoords);	
			else 
				_levelMain.SetCell(0, mapOffset + data.TileMap[i].Offset, data.TileMap[i].TargetId, data.TileMap[i].AtlasCoords);
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

	private bool CheckLrRoom(Vector2I pos)
	{
		if (pos.X - 1 >= 0 && pos.X + 1 < 9 && 
			pos.Y - 1 >= 0 && pos.Y + 1 < 8 &&
			_mapGrid[pos.X, pos.Y] == (int)RoomType.NORMAL && 
			_mapGrid[pos.X - 1, pos.Y] != (int)RoomType.NONE &&
			_mapGrid[pos.X + 1, pos.Y] != (int)RoomType.NONE &&
			_mapGrid[pos.X, pos.Y + 1] == (int)RoomType.NONE &&
			_mapGrid[pos.X, pos.Y - 1] == (int)RoomType.NONE)
		{
			return true;
		}
		return false;
	}
	
	private bool CheckTbRoom(Vector2I pos)
	{
		if (pos.X - 1 >= 0 && pos.X + 1 < 9 && 
			pos.Y - 1 >= 0 && pos.Y + 1 < 8 &&
			_mapGrid[pos.X, pos.Y] == (int)RoomType.NORMAL && 
			_mapGrid[pos.X, pos.Y + 1] != (int)RoomType.NONE &&
			_mapGrid[pos.X, pos.Y - 1] != (int)RoomType.NONE &&
			_mapGrid[pos.X + 1, pos.Y] == (int)RoomType.NONE &&
			_mapGrid[pos.X - 1, pos.Y] == (int)RoomType.NONE)
		{
			return true;
		}
		return false;
	}

	public void PrintMap(Vector2I currentPosition)
	{
		for (int i = 0; i < 8; i++)
		{
			Console.Write("\t");
			for (int j = 0; j < 9; j++)
			{
				if (j == currentPosition.X && i == currentPosition.Y)
					Console.Write(_mapGrid[j, i] + " ");
				else
					Console.Write(_mapGrid[j, i] + " ");
			}

			Console.WriteLine("");
		}
	}
}


