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
	
	private Button _optionButton;
	private Button _creditButton;
	private Button _quitButton;
	
	private Button _optionReturnButton;
	private Button _creditReturnButton;
	
	public override void _Ready()
	{
		_originPosition = Vector2.Zero;
		_originSize = GetViewportRect().Size;
		
		_mainMenu = GetNode<MarginContainer>("MainMenu");
		_optionMenu = GetNode<MarginContainer>("OptionMenu");
		_creditMenu = GetNode<MarginContainer>("CreditMenu");

		_currentContainer = _mainMenu;
		
		// Main Menu Buttons
		_optionButton = _mainMenu.GetNode<Button>("VBoxContainer/OptionButton");
		_creditButton = _mainMenu.GetNode<Button>("VBoxContainer/HBoxContainer/CreditButton");
		_quitButton = _mainMenu.GetNode<Button>("VBoxContainer/HBoxContainer/QuitButton");
		_optionButton.Pressed += MoveToOptionsMenu;
		_creditButton.Pressed += MoveToCreditMenu;
		_quitButton.Pressed += QuitGame;
		
		// Option Menu Buttons
		_optionReturnButton = _optionMenu.GetNode<Button>("ScrollContainer/VBoxContainer/BackButton");
		_optionReturnButton.Pressed += MoveToMainMenu;
		
		// Credit Menu Buttons
		_creditReturnButton = _creditMenu.GetNode<Button>("VBoxContainer/BackButton");
		_creditReturnButton.Pressed += MoveToMainMenu;
	}

	private void QuitGame()
	{
		GetTree().Quit();
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
