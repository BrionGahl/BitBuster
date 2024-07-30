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
	public delegate void BakeNavigationMeshEventHandler(Vector2 position);


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

	public void EmitBakeNavigationMeshSignal(Vector2 position)
	{
		EmitSignal(SignalName.BakeNavigationMesh, position);
	}
}
