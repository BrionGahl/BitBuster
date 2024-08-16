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
	public delegate void BakeNavigationMeshEventHandler();

	[Signal]
	public delegate void SpawnItemEventHandler(Vector2 position, int itemType, int itemIndex);
	
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

	public void EmitBakeNavigationMeshSignal()
	{
		EmitSignal(SignalName.BakeNavigationMesh);
	}

	public void EmitSpawnItemEventHandler(Vector2 position, int itemType, int itemIndex)
	{
		EmitSignal(SignalName.SpawnItem, position, itemType, itemIndex);
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
