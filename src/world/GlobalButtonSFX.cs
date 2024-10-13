using BitBuster.sound;
using BitBuster.utils;
using Godot;

namespace BitBuster.world;

public partial class GlobalButtonSFX : AudioStreamPlayer
{
	public override void _Ready()
	{
		Bus = AudioBuses.SFX;
		Stream = GD.Load<AudioStreamWav>("res://assets/sound/world/global_button_click.wav");
	}
}