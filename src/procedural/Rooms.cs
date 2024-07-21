using System;
using System.Collections.Generic;
using BitBuster.data;
using BitBuster.utils;
using BitBuster.world;
using Godot;

namespace BitBuster.procedural;

public partial class Rooms : Node2D
{
	private RandomNumberGenerator _random;

	public Vector2I RoomSize { get; set; }
	public Vector2I CellSize { get; set; }

	public override void _Notification(int what)
	{
		if (what != NotificationSceneInstantiated) 
			return;
		
		_random = new RandomNumberGenerator();
		_random.Randomize();

		TileMap room = GetChild((int)RoomType.NORMAL - 1).GetChild(0) as TileMap;
		RoomSize = room.GetUsedRect().Size;
		CellSize = room.TileSet.TileSize;
	}
	public RoomData GetRoomData(RoomType type)
	{
		Node2D group = GetChild((int)type - 1) as Node2D; // Grab room type group
		int index = _random.RandiRange(0, group.GetChildCount() - 1); // Randomly select a room
		TileMap room = group.GetChild(index) as TileMap;
		RoomData data = new RoomData();
		
		for (int i = 0; i < room.GetChildCount(); i++)
		{
			data.Objects.Add(room.GetChild(i) as Node2D);
		}

		for (int i = 0; i < room.GetUsedCells(0).Count; i++)
		{
			Vector2I v = room.GetUsedCells(0)[i];

			TileData cellData = room.GetCellTileData(0, v);
			float chance = cellData.GetCustomData("chance").As<float>();
			int targetId = cellData.GetCustomData("target_id").AsInt32();
			Vector2I direction = cellData.GetCustomData("direction").AsVector2I();
			
			int cellSourceId = room.GetCellSourceId(0, v);
			Vector2I atlasCoords = room.GetCellAtlasCoords(0, v);
			
			if (_random.Randf() > chance)
				continue;
			
			data.TileMap.Add(new TileMapData(v, cellSourceId, targetId, atlasCoords, direction));
		}
		
		return data;
	}
	
	
}
