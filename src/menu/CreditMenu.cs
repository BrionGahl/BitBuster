using Godot;

namespace BitBuster.menu;

public partial class CreditMenu : Menu
{
	
	private Button _backButton;

	public override void _Ready()
	{
		_backButton = GetNode<Button>("VBoxContainer/BackButton");
		
		_backButton.Pressed += OnBackPressed;
	}

	private void OnBackPressed()
	{
		EmitSignal(Menu.SignalName.MenuNavigation, this, "mainmenu");
	}
}
