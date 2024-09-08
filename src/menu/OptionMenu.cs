using BitBuster.world;
using Godot;

namespace BitBuster.menu;

public partial class OptionMenu : Menu
{
	private Settings _settings;
	
	// Graphics Tab
	private OptionButton _resolutionOptions;
	private CheckButton _fullscreenToggle;

	private int _baseResolution;
	private bool _baseFullscreenToggle;
	
	private Button _backButton;
	private Button _applyButton;
	
	public override void _Ready()
	{
		base._Ready();

		_settings = new Settings();

		_resolutionOptions = GetNode<OptionButton>("VBoxContainer/TabContainer/Graphics/VBoxContainer/ResolutionSetting/ResolutionOptions");
		_fullscreenToggle = GetNode<CheckButton>("VBoxContainer/TabContainer/Graphics/VBoxContainer/FullscreenToggle");

		_resolutionOptions.Selected = _settings.Resolution;
		_fullscreenToggle.ButtonPressed = _settings.IsFullscreen;

		_baseResolution = _settings.Resolution;
		_baseFullscreenToggle = _settings.IsFullscreen;

		_resolutionOptions.ItemSelected += OnResolutionItemChanged;
		_fullscreenToggle.Pressed += OnFullscreenTogglePressed;

		_applyButton = GetNode<Button>("VBoxContainer/HBoxContainer/ApplyButton");
		_backButton = GetNode<Button>("VBoxContainer/HBoxContainer/BackButton");
		_applyButton.Pressed += OnApplyPressed;
		_backButton.Pressed += OnBackPressed;

	}

	private void OnResolutionItemChanged(long index)
	{
		_settings.Resolution = (int)index;
		_applyButton.Disabled = false;
	}

	private void OnFullscreenTogglePressed()
	{
		_settings.IsFullscreen = _fullscreenToggle.ButtonPressed;
		_applyButton.Disabled = false;
	}
	
	private void OnApplyPressed()
	{
		_settings.SaveSettings();
		
		_baseResolution = _settings.Resolution;
		_baseFullscreenToggle = _settings.IsFullscreen;
	}
	
	private void OnBackPressed()
	{
		GlobalButtonSfx.Play();
		EmitSignal(Menu.SignalName.MenuNavigation, this, "mainmenu");
	}
	
}
