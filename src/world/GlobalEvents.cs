using BitBuster.entity.enemy;
using Godot;

namespace BitBuster.world;

public partial class GlobalEvents : Node
{
	[Signal]
	public delegate void RoomEnterEventHandler(Vector2 position);

	[Signal]
	public delegate void ToggleDoorsEventHandler(bool isOpen);

	[Signal]
	public delegate void IncrementAndGenerateLevelEventHandler();
	
	[Signal]
	public delegate void ClearMinimapEventHandler();
	
	[Signal]
	public delegate void BakeNavigationMeshEventHandler();

	[Signal]
	public delegate void SpawnItemEventHandler(Vector2 position, int itemType, int itemId);
	
	[Signal]
	public delegate void BossRoomEnterEventHandler(Enemy enemy);

	[Signal]
	public delegate void BossKilledEventHandler();
	
	public void EmitRoomEnteredSignal(Vector2 position)
	{
		EmitSignal(SignalName.RoomEnter, position);
	}

	public void EmitToggleDoorsSignal(bool isOpen)
	{
		EmitSignal(SignalName.ToggleDoors, isOpen);
	}

	public void EmitIncrementAndGenerateLevelSignal()
	{
		EmitSignal(SignalName.IncrementAndGenerateLevel);
	}
	
	public void EmitClearMinimapSignal()
	{
		EmitSignal(SignalName.ClearMinimap);
	}

	public void EmitBakeNavigationMeshSignal()
	{
		EmitSignal(SignalName.BakeNavigationMesh);
	}

	public void EmitSpawnItemEventHandler(Vector2 position, int itemType, int itemId)
	{
		EmitSignal(SignalName.SpawnItem, position, itemType, itemId);
	}
	
	public void EmitBossRoomEnterSignal(Enemy enemy)
	{
		EmitSignal(SignalName.BossRoomEnter, enemy);
	}
	
	public void EmitBossKilledSignal()
	{
		EmitSignal(SignalName.BossKilled);
	}
}
