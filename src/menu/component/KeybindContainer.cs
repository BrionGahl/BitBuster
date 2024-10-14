using BitBuster.world;
using Godot;
using Godot.Collections;

namespace BitBuster.menu.component;

public partial class KeybindContainer : HBoxContainer
{
	[Signal]
	public delegate void KeybindButtonPressedEventHandler(KeybindContainer currContainer);
	
	private Dictionary<string, InputEvent> _keybindDict;
	
	private Label _inputAction;
	private Button _inputButton;

	public string ActionText
	{
		get => _inputAction.Text;
		set => _inputAction.Text = value;
	}

	public string ButtonText
	{
		get => _inputButton.Text;
		set => _inputButton.Text = value;
	}
	
	public override void _Ready()
	{
		_inputAction = GetNode<Label>("InputAction");
		_inputButton = GetNode<Button>("InputButton");

		_inputButton.Pressed += OnButtonPressed;
	}

	private void OnButtonPressed()
	{
		EmitSignal(KeybindContainer.SignalName.KeybindButtonPressed, this);
	}
}
