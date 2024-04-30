using BitBuster.utils;
using BitBuster.world;
using Godot;

namespace BitBuster.tiles;

public partial class PlayerDetector : Area2D
{
	private GlobalEvents _globalEvents;
	
	public override void _Ready()
	{
		_globalEvents = GetNode<GlobalEvents>("/root/GlobalEvents");
		this.BodyEntered += OnBodyEntered;
	}
	
	private void OnBodyEntered(Node2D body)
	{
		if (!body.IsInGroup("player"))
			return;
		
		Logger.Log.Information("Player entered PlayerDetector in room at " + GlobalPosition);
		_globalEvents.EmitRoomEnteredSignal(GlobalPosition);
	}
}
