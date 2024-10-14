using BitBuster.world;
using Godot;
using Godot.Collections;

namespace BitBuster.menu;

public partial class OptionMenu : Menu
{
	private Settings _settings;

	private Dictionary<string, Variant> _videoDict;
	private Dictionary<string, Variant> _audioDict;
	private Dictionary<string, InputEvent> _keyDict;
	
	// Video
	private OptionButton _resolutionOptions;
	private CheckBox _fullscreenToggle;
	private CheckBox _vsyncToggle;
	
	// Audio
	private Label _masterVolPercentLabel;
	private Label _sfxVolPercentLabel;
	private Label _musicVolPercentLabel;
	private HSlider _masterVolSlider;
	private HSlider _sfxVolSlider;
	private HSlider _musicVolSlider;


	private int _baseResolution;
	private bool _baseFullscreenToggle;
	
	private Button _backButton;
	
	public override void _Ready()
	{
		base._Ready();

		_settings = GetNode<Settings>("/root/Settings");

		_videoDict = _settings.LoadVideoSettings();
		_audioDict = _settings.LoadAudioSettings();
		_keyDict = _settings.LoadKeybindingsSettings();
		
		// Video
		_resolutionOptions = GetNode<OptionButton>("VBoxContainer/TabContainer/Video/VBoxContainer/ResolutionHBox/ResolutionOptions");
		_fullscreenToggle = GetNode<CheckBox>("VBoxContainer/TabContainer/Video/VBoxContainer/FullscreenHBox/FullscreenCheckBox");
		_vsyncToggle = GetNode<CheckBox>("VBoxContainer/TabContainer/Video/VBoxContainer/VSyncHBox/VSyncCheckBox");
		_resolutionOptions.Selected = _videoDict["resolution"].AsInt32();
		_fullscreenToggle.ButtonPressed = _videoDict["fullscreen"].AsBool();
		_vsyncToggle.ButtonPressed = _videoDict["vsync"].AsBool();
		
		_fullscreenToggle.Toggled += OnFullscreenToggle;
		_resolutionOptions.ItemSelected += OnResolutionItemChanged;
		_vsyncToggle.Toggled += OnVsyncToggled;

		// Audio
		_masterVolPercentLabel = GetNode<Label>("VBoxContainer/TabContainer/Audio/VBoxContainer/MasterVolHBox/PercentLabel");
		_sfxVolPercentLabel = GetNode<Label>("VBoxContainer/TabContainer/Audio/VBoxContainer/SfxVolHBox/PercentLabel");
		_musicVolPercentLabel = GetNode<Label>("VBoxContainer/TabContainer/Audio/VBoxContainer/MusicVolHBox/PercentLabel");
		_masterVolSlider = GetNode<HSlider>("VBoxContainer/TabContainer/Audio/VBoxContainer/MasterVolHBox/HSlider");
		_sfxVolSlider = GetNode<HSlider>("VBoxContainer/TabContainer/Audio/VBoxContainer/SfxVolHBox/HSlider");
		_musicVolSlider= GetNode<HSlider>("VBoxContainer/TabContainer/Audio/VBoxContainer/MusicVolHBox/HSlider");

		_masterVolPercentLabel.Text = $"{_audioDict["master_volume"].AsDouble() * 100}%";
		_sfxVolPercentLabel.Text = $"{_audioDict["sfx_volume"].AsDouble() * 100}%";
		_musicVolPercentLabel.Text = $"{_audioDict["music_volume"].AsDouble() * 100}%";
		_masterVolSlider.Value = Mathf.Min(_audioDict["master_volume"].AsDouble(), 1.0) * 100;
		_sfxVolSlider.Value = Mathf.Min(_audioDict["sfx_volume"].AsDouble(), 1.0) * 100;
		_musicVolSlider.Value = Mathf.Min(_audioDict["music_volume"].AsDouble(), 1.0) * 100;

		_masterVolSlider.DragEnded += OnMasterVolChanged;
		_sfxVolSlider.DragEnded += OnSfxVolChanged;
		_musicVolSlider.DragEnded += OnMusicVolChanged;
		
		_backButton = GetNode<Button>("VBoxContainer/BackButton");
		_backButton.Pressed += OnBackPressed;

	}

	private void OnResolutionItemChanged(long index)
	{
		_settings.SaveVideoSetting("resolution", index);
	}

	private void OnFullscreenToggle(bool toggleOn)
	{
		_settings.SaveVideoSetting("fullscreen", toggleOn);
	}

	private void OnVsyncToggled(bool toggleOn)
	{
		_settings.SaveVideoSetting("vsync", toggleOn);
	}

	private void OnMasterVolChanged(bool isChanged)
	{
		if (!isChanged) 
			return;
		_settings.SaveAudioSetting("master_volume", _masterVolSlider.Value / 100);
		_masterVolPercentLabel.Text = $"{_masterVolSlider.Value}%";
	}

	private void OnSfxVolChanged(bool isChanged)
	{
		if (!isChanged) 
			return;
		_settings.SaveAudioSetting("sfx_volume", _sfxVolSlider.Value / 100);
		_sfxVolPercentLabel.Text = $"{_sfxVolSlider.Value}%";
	}
	
	private void OnMusicVolChanged(bool isChanged)
	{
		if (!isChanged) 
			return;
		_settings.SaveAudioSetting("music_volume", _musicVolSlider.Value / 100);
		_musicVolPercentLabel.Text = $"{_musicVolSlider.Value}%";
	}

	private void OnBackPressed()
	{
		GlobalButtonSfx.Play();
		EmitSignal(Menu.SignalName.MenuNavigation, this, "mainmenu");
	}
}
