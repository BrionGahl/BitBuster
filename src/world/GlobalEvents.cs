using Godot;

namespace BitBuster.world;

public partial class GlobalEvents : Node
{
	[Signal]
	public delegate void RoomEnterEventHandler(Vector2 position);

	[Signal]
	public delegate void ToggleDoorsEventHandler(bool isOpen);

	public void EmitRoomEnteredSignal(Vector2 position)
	{
		EmitSignal(SignalName.RoomEnter, position);
	}

	public void EmitToggleDoorsSignal(bool isOpen)
	{
		EmitSignal(SignalName.ToggleDoors, isOpen);
	}
}
