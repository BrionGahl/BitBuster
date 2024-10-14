using Godot;

namespace BitBuster.menu;

public partial class CreditMenu : Menu
{
	
	private Button _backButton;

	public override void _Ready()
	{
		base._Ready();

		_backButton = GetNode<Button>("VBoxContainer/BackButton");
		
		_backButton.Pressed += OnBackPressed;
	}

	private void OnBackPressed()
	{
		GlobalButtonSfx.Play();
		EmitSignal(Menu.SignalName.MenuNavigation, this, "mainmenu");
	}
}
