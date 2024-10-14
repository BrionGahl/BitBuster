using BitBuster.utils;
using BitBuster.world;
using Godot;

namespace BitBuster.menu.other;

public partial class BaseMenu : Menu
{
	[Signal]
	public delegate void TogglePauseEventHandler();

	private Global _global;
	
	private Button _continueButton;
	private Button _optionButton;
	private Button _returnButton;

	public override void _Ready()
	{
		base._Ready();

		_global = GetNode<Global>("/root/Global");
		
		_continueButton = GetNode<Button>("VBoxContainer/ContinueButton");
		_optionButton = GetNode<Button>("VBoxContainer/OptionsButton");
		_returnButton = GetNode<Button>("VBoxContainer/ReturnButton");

		_continueButton.Pressed += OnContinuePressed;
		_optionButton.Pressed += OnOptionPressed;
		_returnButton.Pressed += OnReturnPressed;
	}
	
	private void OnContinuePressed()
	{
		Logger.Log.Information("Toggling Pause...");
		GlobalButtonSfx.Play();
		EmitSignal(SignalName.TogglePause);
	}
	
	private void OnOptionPressed()
	{
		Logger.Log.Information("Opening Options...");
		GlobalButtonSfx.Play();
		EmitSignal(Menu.SignalName.MenuNavigation, this, "optionmenu");
	}
	
	private void OnReturnPressed()
	{
		Logger.Log.Information("Returning to Main Menu...");
		GlobalButtonSfx.Play();
		GetTree().Paused = false;
		GetTree().ChangeSceneToPacked(_global.MainMenuPackedScene);
	}
}
