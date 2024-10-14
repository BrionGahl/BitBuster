using BitBuster.utils;
using BitBuster.world;
using Godot;

namespace BitBuster.menu;

public partial class PlayMenu : Menu
{
	private Global _global;

	private Button _playButton;
	private Button _continueButton;
	private Button _backButton;

	public override void _Ready()
	{
		base._Ready();

		_global = GetNode<Global>("/root/Global");

		_playButton = GetNode<Button>("VBoxContainer/PlayButton");
		_continueButton = GetNode<Button>("VBoxContainer/ContinueButton");
		_backButton = GetNode<Button>("VBoxContainer/BackButton");

		_playButton.Pressed += OnPlayPressed;
		_backButton.Pressed += OnBackPressed;
	}

	private void OnPlayPressed()
	{
		Logger.Log.Information("Moving to Game and Level Generation...");
		GlobalButtonSfx.Play();
		GetTree().ChangeSceneToPacked(_global.GamePackedScene);
	}
	
	private void OnBackPressed()
	{
		GlobalButtonSfx.Play();
		EmitSignal(Menu.SignalName.MenuNavigation, this, "mainmenu");
	}
}
