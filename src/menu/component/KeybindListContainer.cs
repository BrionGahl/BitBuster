using System.Collections.Generic;
using BitBuster.utils;
using BitBuster.world;
using Godot;

namespace BitBuster.menu.component;

public partial class KeybindListContainer : VBoxContainer
{
	private Settings _settings;
	private Godot.Collections.Dictionary<string, InputEvent> _keybindDict;

	private PackedScene _keybindContainer;

	private KeybindContainer _currentMappingContainer;
	private bool _isRemapping;
	
	public override void _Ready()
	{
		_settings = GetNode<Settings>("/root/Settings");
		_keybindDict = _settings.LoadKeybindingsSettings();

		_keybindContainer = GD.Load<PackedScene>("res://scenes/subscenes/menu/component/keybind_container.tscn");
		
		CreateKeybindList();
	}

	private void CreateKeybindList()
	{
		foreach (KeyValuePair<string, InputEvent> var in _keybindDict)
		{
			KeybindContainer container = _keybindContainer.Instantiate<KeybindContainer>();
			
			Label actionLabel = container.FindChild("InputAction") as Label;
			Button inputButton = container.FindChild("InputButton") as Button;

			actionLabel.Text = var.Key;
			inputButton.Text = SetInputButtonTextFromInputEvent(var.Value);
			AddChild(container);
			container.KeybindButtonPressed += OnKeybindButtonPressed;
		}
	}
	
	private string SetInputButtonTextFromInputEvent(InputEvent @event) 
	{
		if (@event is InputEventKey inputEventKey)
			return OS.GetKeycodeString(inputEventKey.Keycode);
		
		if (@event is InputEventMouseButton inputEventMouseButton)
		{
			return $"mouse_{inputEventMouseButton.ButtonIndex.ToString()}";
		}

		return "";
	}
	
	private void OnKeybindButtonPressed(KeybindContainer container)
	{
		if (_isRemapping)
			return;
		
		_isRemapping = true;

		_currentMappingContainer = container;
		container.ButtonText = "Press Key to Bind...";
		
	}

	public override void _Input(InputEvent @event)
	{
		if (!_isRemapping)
			return;

		if ((@event is InputEventKey || @event is InputEventMouseButton) && @event.IsPressed())
		{
			InputMap.ActionEraseEvents(_currentMappingContainer.ActionText);
			InputMap.ActionAddEvent(_currentMappingContainer.ActionText, @event);

			_currentMappingContainer.ButtonText = SetInputButtonTextFromInputEvent(@event);
			
			_settings.SaveKeybindingsSetting(_currentMappingContainer.ActionText, @event);
			
			_isRemapping = false;
		}
	}
}
