using Godot;

namespace BitBuster.world;

public partial class GlobalButtonSFX : AudioStreamPlayer
{
	public override void _Ready()
	{
		Stream = GD.Load<AudioStreamWav>("res://assets/sound/world/global_button_click.wav");
	}
}