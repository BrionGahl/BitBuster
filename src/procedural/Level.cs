using Godot;

namespace BitBuster.procedural;

public partial class Level : Node2D
{
	
	// TODO: Will catch a signal for player death and handle moving to main menu
	
	private Node2D Extra { get; set; }

	
	public override void _Ready()
	{
		Extra = GetNode<Node2D>("Extra");
		// for (int i = 0; i < Extra.GetChildCount(); i++)
		// {
		// 	
		// }
	}
}
