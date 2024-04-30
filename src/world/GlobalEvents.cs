using Godot;

namespace BitBuster.world;

public partial class GlobalEvents : Node
{
	[Signal]
	public delegate void RoomEnterEventHandler(Vector2 position);


	public void EmitRoomEnteredSignal(Vector2 position)
	{
		EmitSignal(SignalName.RoomEnter, position);
	}
}
