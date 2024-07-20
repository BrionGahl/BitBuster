using System.Collections.Generic;
using BitBuster.entity.enemy;
using BitBuster.utils;
using BitBuster.world;
using Godot;

namespace BitBuster.tiles;

public partial class EntityDetector : Area2D
{
	private GlobalEvents _globalEvents;

	public int EnemyCount { get; set; }
	
	public override void _Ready()
	{
		_globalEvents = GetNode<GlobalEvents>("/root/GlobalEvents");

		BodyEntered += OnBodyEntered;
		BodyExited += OnBodyExited;
	}
	
	private void OnBodyEntered(Node2D body)
	{
		if (!body.IsInGroup("player"))
			return;
		
		foreach (var entity in GetOverlappingBodies())
		{
			if (entity.IsInGroup("enemy"))
				EnemyCount++;
		}

		if (EnemyCount > 0)
		{
			Logger.Log.Information("Closing doors...");
			_globalEvents.EmitToggleDoorsSignal(false);
		}
		
		Logger.Log.Information("Player entered PlayerDetector in room at {@GlobalPosition}.", GlobalPosition.ToString());
		Logger.Log.Information("Number of enemies in room: {@EnemyCount}", EnemyCount);

		_globalEvents.EmitRoomEnteredSignal(GlobalPosition);
	}

	private void OnBodyExited(Node2D body)
	{
		if (!body.IsInGroup("enemy"))
		{
			return;
		}
		
		Logger.Log.Information("Enemy removed from room...");
		EnemyCount--;

		if (EnemyCount <= 0)
		{
			Logger.Log.Information("Opening doors...");
			_globalEvents.EmitToggleDoorsSignal(true);
		}
	}
}
