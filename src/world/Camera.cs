using BitBuster.utils;
using Godot;

namespace BitBuster.world;

public partial class Camera : Camera2D
{

	private GlobalEvents _globalEvents;
	
	public override void _Ready()
	{
		_globalEvents = GetNode<GlobalEvents>("/root/GlobalEvents");
		
		_globalEvents.RoomEnter += OnPlayerEnterRoom;
	}

	private void OnPlayerEnterRoom(Vector2 position)
	{
		Logger.Log.Information("Moving camera to global position: " + position);
		GlobalPosition = position;
	}
}
