using System.Collections.Generic;
using BitBuster.sound;
using BitBuster.utils;
using Godot;

namespace BitBuster.world;

public partial class Settings : Node
{
	private const string SettingsFilePath = "user://settings.ini";
	private ConfigFile _configFile;
	private string _loadVersion;
	
	private static readonly Godot.Collections.Dictionary<int, Vector2I> ResolutionDict = new Godot.Collections.Dictionary<int, Vector2I>
	{
		{0, new Vector2I(640, 320)},
		{1, new Vector2I(1280, 720)},
		{2, new Vector2I(1920, 1080)},
		{3, new Vector2I(2560, 1440)},
		{4, new Vector2I(3840, 2160)}
	};
	
	public override void _Ready()
	{
		_configFile = new ConfigFile();
		_loadVersion = ProjectSettings.GetSettingWithOverride("application/config/version").AsString();
		
		if (!FileAccess.FileExists(SettingsFilePath))
		{
			_configFile.SetValue("debug", "last_load_version", ProjectSettings.GetSettingWithOverride("application/config/version").AsString());
			ResetSettings(true, true, true);
			_configFile.Save(SettingsFilePath);
		}

		_configFile.Load(SettingsFilePath);

		if (!_configFile.HasSection("debug") || !_configFile.GetValue("debug", "last_load_version").AsString().Equals(_loadVersion))
		{
			Logger.Log.Information("Debug version doesn't match or is missing... regenerating settings config...");
			_configFile.Clear();
			_configFile.SetValue("debug", "last_load_version", ProjectSettings.GetSettingWithOverride("application/config/version").AsString());
			ResetSettings(true, true, true);
			_configFile.Save(SettingsFilePath);
		}
		
		ApplyVideoSettings();
		ApplyAudioSettings();
		ApplyKeybindings();
	}

	public void ResetSettings(bool video, bool audio, bool keybindings)
	{
		if (video)
			ResetVideo();
		
		if (audio)
			ResetAudio();
		
		if (keybindings)
			ResetKeybindings();
	}

	private void ResetVideo()
	{
		_configFile.SetValue("video", "fullscreen", true);
		_configFile.SetValue("video", "resolution", 0);
		_configFile.SetValue("video", "vsync", true);
	}

	private void ResetAudio()
	{
		_configFile.SetValue("audio", "master_volume", 0.5);
		_configFile.SetValue("audio", "sfx_volume", 0.5);
		_configFile.SetValue("audio", "music_volume", 0.5);
	}

	private void ResetKeybindings()
	{
		_configFile.SetValue("keybindings", "up", "W");
		_configFile.SetValue("keybindings", "left", "A");
		_configFile.SetValue("keybindings", "down", "S");
		_configFile.SetValue("keybindings", "right", "D");
		_configFile.SetValue("keybindings", "shoot", "mouse_1");
		_configFile.SetValue("keybindings", "use", "mouse_2");
		_configFile.SetValue("keybindings", "interact", "space");
		_configFile.SetValue("keybindings", "bomb", "Q");
	}
	
	public void SaveVideoSetting(string key, Variant value)
	{
		_configFile.SetValue("video", key, value);
		_configFile.Save(SettingsFilePath);
		ApplyVideoSettings();
	}

	public Godot.Collections.Dictionary<string, Variant> LoadVideoSettings()
	{
		Godot.Collections.Dictionary<string, Variant> videoSettings = new Godot.Collections.Dictionary<string, Variant>();
		foreach (string key in _configFile.GetSectionKeys("video"))
		{
			videoSettings[key] = _configFile.GetValue("video", key);
		}

		return videoSettings;
	}
	
	public void SaveAudioSetting(string key, Variant value)
	{
		_configFile.SetValue("audio", key, value);
		_configFile.Save(SettingsFilePath);
		ApplyAudioSettings();
	}

	public Godot.Collections.Dictionary<string, Variant> LoadAudioSettings()
	{
		Godot.Collections.Dictionary<string, Variant> audioSettings = new Godot.Collections.Dictionary<string, Variant>();
		foreach (string key in _configFile.GetSectionKeys("audio"))
		{
			audioSettings[key] = _configFile.GetValue("audio", key);
		}

		return audioSettings;
	}
	
	public void SaveKeybindingsSetting(string key, InputEvent value)
	{
		string eventStr = "";
		if (value is InputEventKey inputEventKey)
			eventStr = OS.GetKeycodeString(inputEventKey.PhysicalKeycode);
		else if (value is InputEventMouseButton inputEventMouseButton)
			eventStr = $"mouse_{inputEventMouseButton.ButtonIndex.ToString()}";
		
		_configFile.SetValue("keybindings", key, eventStr);
		_configFile.Save(SettingsFilePath);
		ApplyKeybindings();
	}

	public Godot.Collections.Dictionary<string, InputEvent> LoadKeybindingsSettings()
	{
		Godot.Collections.Dictionary<string, InputEvent> keybindingsSettings = new Godot.Collections.Dictionary<string, InputEvent>();
		foreach (string key in _configFile.GetSectionKeys("keybindings"))
		{
			InputEvent inputEvent;
			string eventStr = _configFile.GetValue("keybindings", key).AsString();
			
			if (eventStr.Contains("mouse_"))
			{
				inputEvent = new InputEventMouseButton();
				((InputEventMouseButton)inputEvent).ButtonIndex = (MouseButton)eventStr.Split("_")[1].ToInt();
			}
			else
			{
				inputEvent = new InputEventKey();
				((InputEventKey)inputEvent).Keycode = OS.FindKeycodeFromString(eventStr);
			}
			
			keybindingsSettings[key] = inputEvent;
		}

		return keybindingsSettings;
	}

	private void ApplyVideoSettings()
	{
		DisplayServer.WindowSetSize(ResolutionDict[_configFile.GetValue("video", "resolution").AsInt32()]);
		DisplayServer.WindowSetMode(
			_configFile.GetValue("video", "fullscreen").AsBool() 
				? DisplayServer.WindowMode.Fullscreen 
				: DisplayServer.WindowMode.Windowed
		);
		DisplayServer.WindowSetVsyncMode(
			_configFile.GetValue("video", "vsync").AsBool() 
				? DisplayServer.VSyncMode.Enabled 
				: DisplayServer.VSyncMode.Disabled
		);
	}

	private void ApplyAudioSettings()
	{
		AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex(AudioBuses.Master), Mathf.LinearToDb(_configFile.GetValue("audio", "master_volume").AsSingle()));
		AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex(AudioBuses.SFX), Mathf.LinearToDb(_configFile.GetValue("audio", "sfx_volume").AsSingle()));
		AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex(AudioBuses.Music), Mathf.LinearToDb(_configFile.GetValue("audio", "music_volume").AsSingle()));
	}

	private void ApplyKeybindings()
	{
		foreach (KeyValuePair<string, InputEvent> var in LoadKeybindingsSettings())
		{
			InputMap.ActionEraseEvents(var.Key);
			InputMap.ActionAddEvent(var.Key, var.Value);
		}
	}
}
