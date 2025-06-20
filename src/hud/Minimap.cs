using BitBuster.procedural;
using BitBuster.utils;
using BitBuster.world;
using Godot;

namespace BitBuster.hud;

public partial class Minimap: TextureRect
{
	private Global _global;
	private GlobalEvents _globalEvents;

	private Control _roomsOnFloor;
	private Control _roomChoices;

	private TextureRect _currPos;
	
	private int[,] _exploredMapGrid;

	public override void _Ready()
	{
		_global = GetNode<Global>("/root/Global");
		_globalEvents = GetNode<GlobalEvents>("/root/GlobalEvents");
		
		_roomsOnFloor = GetNode<Control>("RoomsOnFloor");
		_roomChoices = GetNode<Control>("RoomChoices");

		_currPos = GetNode<TextureRect>("RoomsOnFloor/CurrentPos");
		
		_exploredMapGrid = new int[9, 10];
		
		_globalEvents.RoomEnter += OnPlayerEnterRoom;
		_globalEvents.ClearMinimap += OnClearMinimap;
	}

	private void AddUnknownRoom(Vector2I position, RoomType room)
	{
		_exploredMapGrid[position.X, position.Y] = (int)room;
		
		TextureRect roomToAdd = _roomChoices.GetChild((int)room).Duplicate() as TextureRect;
		roomToAdd.Position = position * 8;
		roomToAdd.Modulate = new Color(Colors.White, 0.7f);
		_roomsOnFloor.AddChild(roomToAdd);
	}
	
	private void OnPlayerEnterRoom(Vector2 position)
	{
		TextureRect roomToAdd;

		Vector2I adjustedPos = (Vector2I) (position / 320).Floor();
		int currRoomType = _global.MapGrid[adjustedPos.X, adjustedPos.Y];
		Logger.Log.Information("Room Enter Signal received from Minimap... Calculating room for {@posVec}", adjustedPos.ToString());

		
		_exploredMapGrid[adjustedPos.X, adjustedPos.Y] = currRoomType;
		
		roomToAdd = _roomChoices.GetChild(currRoomType).Duplicate() as TextureRect;
		roomToAdd.Position = adjustedPos * 8;
		roomToAdd.Modulate = new Color(Colors.White);
		_currPos.Position = adjustedPos * 8;

		
		_roomsOnFloor.AddChild(roomToAdd);
		
		if (_global.MapGrid[adjustedPos.X + 1, adjustedPos.Y] != (int)RoomType.None && _exploredMapGrid[adjustedPos.X + 1, adjustedPos.Y] == 0)
		{
			AddUnknownRoom(new Vector2I(adjustedPos.X + 1, adjustedPos.Y), (RoomType)_global.MapGrid[adjustedPos.X + 1, adjustedPos.Y]);
		}
		if (_global.MapGrid[adjustedPos.X - 1, adjustedPos.Y] != (int)RoomType.None && _exploredMapGrid[adjustedPos.X - 1, adjustedPos.Y] == 0)
		{
			AddUnknownRoom(new Vector2I(adjustedPos.X - 1, adjustedPos.Y), (RoomType)_global.MapGrid[adjustedPos.X - 1, adjustedPos.Y]);
		}
		if (_global.MapGrid[adjustedPos.X, adjustedPos.Y + 1] != (int)RoomType.None && _exploredMapGrid[adjustedPos.X, adjustedPos.Y + 1] == 0)
		{
			AddUnknownRoom(new Vector2I(adjustedPos.X, adjustedPos.Y + 1), (RoomType)_global.MapGrid[adjustedPos.X, adjustedPos.Y + 1]);
		}
		if (_global.MapGrid[adjustedPos.X, adjustedPos.Y - 1] != (int)RoomType.None && _exploredMapGrid[adjustedPos.X, adjustedPos.Y - 1] == 0)
		{
			AddUnknownRoom(new Vector2I(adjustedPos.X, adjustedPos.Y - 1), (RoomType)_global.MapGrid[adjustedPos.X, adjustedPos.Y - 1]);
		}
	}

	private void OnClearMinimap()
	{
		_exploredMapGrid = new int[9, 8];
		
		foreach (Node child in _roomsOnFloor.GetChildren())
		{
			if (child.IsInGroup(Groups.GroupCurrentPosition))
				continue;
			child.QueueFree();
		}
	} 
	
	public override void _ExitTree()
	{
		_globalEvents.RoomEnter -= OnPlayerEnterRoom;
		_globalEvents.ClearMinimap -= OnClearMinimap;	
	}
}
