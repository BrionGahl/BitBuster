using System.Collections.Generic;
using BitBuster.entity.enemy;
using BitBuster.utils;
using BitBuster.world;
using Godot;

namespace BitBuster.tiles;

public partial class PlayerDetector : Area2D
{
	private GlobalEvents _globalEvents;

	// private NavigationRegion2D _navigationRegion;
	
	public override void _Ready()
	{
		_globalEvents = GetNode<GlobalEvents>("/root/GlobalEvents");
		// _navigationRegion = GetNode<NavigationRegion2D>("NavRegion");
		this.BodyEntered += OnBodyEntered;
		// this.BodyExited += OnBodyExited;
	}
	
	private void OnBodyEntered(Node2D body)
	{
		if (!body.IsInGroup("player"))
			return;
		
		Logger.Log.Information("Player entered PlayerDetector in room at " + GlobalPosition);

		// _navigationRegion.Enabled = true;
		_globalEvents.EmitRoomEnteredSignal(GlobalPosition);
	}
	
	// private void OnBodyExited(Node2D body)
	// {
	// 	if (!body.IsInGroup("player"))
	// 		return;
	// 	
	// 	Logger.Log.Information("Player exited PlayerDetector in room at " + GlobalPosition);
	// 	_navigationRegion.Enabled = false;
	// }
}
