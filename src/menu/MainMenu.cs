using BitBuster.utils;
using Godot;

namespace BitBuster.menu;

public partial class MainMenu : Menu
{
	private Button _playButton;
	private Button _optionButton;
	private Button _creditButton;
	private Button _quitButton;

	private Label _versionLabel;
	
	public override void _Ready()
	{
		_playButton = GetNode<Button>("VBoxContainer/PlayButton");
		_optionButton = GetNode<Button>("VBoxContainer/HBoxContainer/OptionButton");
		_creditButton = GetNode<Button>("VBoxContainer/CreditButton");
		_quitButton = GetNode<Button>("VBoxContainer/HBoxContainer/QuitButton");

		_versionLabel = GetNode<Label>("VBoxContainer/VersionContainer/VersionLabel");
		_versionLabel.Text = $"ver. {(string)ProjectSettings.GetSettingWithOverride("application/config/version")}";
		
		_playButton.Pressed += OnPlayPressed;
		_optionButton.Pressed += OnOptionPressed;
		_creditButton.Pressed += OnCreditPressed;
		_quitButton.Pressed += OnQuitPressed;
	}
	
	private void OnQuitPressed()
	{
		Logger.Log.Information("Closing Game...");
		GetTree().Quit();
	}
	
	private void OnPlayPressed()
	{
		EmitSignal(Menu.SignalName.MenuNavigation, this, "playmenu");
	}
	
	private void OnOptionPressed()
	{
		EmitSignal(Menu.SignalName.MenuNavigation, this, "optionmenu");
	}
	
	private void OnCreditPressed()
	{
		EmitSignal(Menu.SignalName.MenuNavigation, this, "creditmenu");
	}
}
