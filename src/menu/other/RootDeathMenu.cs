using BitBuster.entity.player;
using BitBuster.utils;
using BitBuster.world;
using Godot;

namespace BitBuster.menu.other;

public partial class RootDeathMenu : Control
{
	private Player _player;
	private Global _global;
	private GlobalButtonSFX _globalButtonSfx;

	private Button _restartButton;
	private Button _returnButton;

	private AudioStreamPlayer _gameoverSound;
	
	public override void _Ready()
	{
		Visible = false;
		
		_player = (Player)GetTree().GetFirstNodeInGroup("player");
		_global = GetNode<Global>("/root/Global");
		_globalButtonSfx = GetNode<GlobalButtonSFX>("/root/GlobalButtonSfx");
		
		_restartButton = GetNode<Button>("MarginContainer/VBoxContainer/HBoxContainer/RestartButton");
		_returnButton = GetNode<Button>("MarginContainer/VBoxContainer/HBoxContainer/ReturnButton");

		_gameoverSound = GetNode<AudioStreamPlayer>("GameoverSound");
		
		_player.Died += OnPlayerDied;
		
		_restartButton.Pressed += OnRestartPressed;
		_returnButton.Pressed += OnReturnPressed;
	}

	private void OnPlayerDied()
	{
		_gameoverSound.Play();
		Visible = true;
	}

	private void OnRestartPressed()
	{
		Logger.Log.Information("Restart pressed...");
		_globalButtonSfx.Play();
		GetTree().ChangeSceneToPacked(_global.GamePackedScene);
	}

	private void OnReturnPressed()
	{
		Logger.Log.Information("Return to Menu pressed...");
		_globalButtonSfx.Play();
		GetTree().ChangeSceneToPacked(_global.MainMenuPackedScene);
	}
}
