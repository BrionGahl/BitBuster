using BitBuster.utils;
using Godot;
using Godot.Collections;

namespace BitBuster.menu;

public partial class RootMenu : Control
{

	private Vector2 _originPosition;
	private Vector2 _originSize;

	private MarginContainer _currentContainer;
	
	private MarginContainer _mainMenu;
	private MarginContainer _optionMenu;
	private MarginContainer _creditMenu;
	private MarginContainer _playMenu;

	private Button _playButton;
	private Button _optionButton;
	private Button _creditButton;
	private Button _quitButton;
	
	private Button _optionReturnButton;
	private Button _creditReturnButton;
	private Button _playReturnButton;
	private Button _playDeployButton;

	private PackedScene _mainGamePackedScene;
	
	public override void _Ready()
	{
		_originPosition = Vector2.Zero;
		_originSize = GetViewportRect().Size;
		
		_mainMenu = GetNode<MarginContainer>("MainMenu");
		_optionMenu = GetNode<MarginContainer>("OptionMenu");
		_creditMenu = GetNode<MarginContainer>("CreditMenu");
		_playMenu = GetNode<MarginContainer>("PlayMenu");
		
		_currentContainer = _mainMenu;
		
		// Main Menu Buttons
		_playButton = _mainMenu.GetNode<Button>("VBoxContainer/PlayButton");
		_optionButton = _mainMenu.GetNode<Button>("VBoxContainer/HBoxContainer/OptionButton");
		_creditButton = _mainMenu.GetNode<Button>("VBoxContainer/CreditButton");
		_quitButton = _mainMenu.GetNode<Button>("VBoxContainer/HBoxContainer/QuitButton");
		_playButton.Pressed += MoveToPlayMenu;
		_optionButton.Pressed += MoveToOptionsMenu;
		_creditButton.Pressed += MoveToCreditMenu;
		_quitButton.Pressed += QuitGame;
	
		// Play Menu Buttons
		_playDeployButton = _playMenu.GetNode<Button>("VBoxContainer/PlayButton");
		_playReturnButton = _playMenu.GetNode<Button>("VBoxContainer/BackButton");
		_playDeployButton.Pressed += StartGame;
		_playReturnButton.Pressed += MoveToMainMenu;
		
		// Option Menu Buttons
		_optionReturnButton = _optionMenu.GetNode<Button>("ScrollContainer/VBoxContainer/BackButton");
		_optionReturnButton.Pressed += MoveToMainMenu;
		
		// Credit Menu Buttons
		_creditReturnButton = _creditMenu.GetNode<Button>("VBoxContainer/BackButton");
		_creditReturnButton.Pressed += MoveToMainMenu;

		_mainGamePackedScene = ResourceLoader.Load<PackedScene>("res://scenes/subscenes/world/world.tscn");
		
		Logger.Log.Information("RootMenu is Ready!");
	}

	private void StartGame()
	{
		Logger.Log.Information("Moving to Game and Level Generation...");
		GetTree().ChangeSceneToPacked(_mainGamePackedScene);
	}
	
	private void QuitGame()
	{
		Logger.Log.Information("Closing Game...");
		GetTree().Quit();
	}
	
	private void MoveToPlayMenu()
	{
		_currentContainer.GlobalPosition = _playMenu.GlobalPosition;
		_playMenu.GlobalPosition = _originPosition;

		_currentContainer = _playMenu;
	}
	
	private void MoveToOptionsMenu()
	{
		_currentContainer.GlobalPosition = _optionMenu.GlobalPosition;
		_optionMenu.GlobalPosition = _originPosition;

		_currentContainer = _optionMenu;
	}
	
	private void MoveToCreditMenu()
	{
		_currentContainer.GlobalPosition = _creditMenu.GlobalPosition;
		_creditMenu.GlobalPosition = _originPosition;

		_currentContainer = _creditMenu;
	}

	private void MoveToMainMenu()
	{
		_currentContainer.GlobalPosition = _mainMenu.GlobalPosition;
		_mainMenu.GlobalPosition = _originPosition;

		_currentContainer = _mainMenu;
	}
}
