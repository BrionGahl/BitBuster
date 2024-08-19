using System.Collections.Generic;
using BitBuster.utils;
using Godot;

namespace BitBuster.world;

public class Settings
{
	private const string SettingsPath = "user://settings.ini";
	private ConfigFile _configFile;

	private static readonly Dictionary<int, Vector2I> ResolutionDict = new Dictionary<int, Vector2I>
	{
		{0, new Vector2I(640, 320)},
		{1, new Vector2I(1280, 720)},
		{2, new Vector2I(1920, 1080)},
		{3, new Vector2I(2560, 1440)},
		{4, new Vector2I(3840, 2160)}
	};

	public bool IsFullscreen
	{
		get => (bool)_configFile.GetValue("graphics", "fullscreen");
		set => _configFile.SetValue("graphics", "fullscreen", value);
	}

	public int Resolution
	{
		get => (int)_configFile.GetValue("graphics", "resolution");
		set => _configFile.SetValue("graphics", "resolution", value);
	}

	public Settings()
	{
		_configFile = new ConfigFile();
		_configFile.Load(SettingsPath);

		if (!_configFile.HasSectionKey("graphics", "resolution"))
			_configFile.SetValue("graphics", "resolution", 0);

		if (!_configFile.HasSectionKey("graphics", "fullscreen"))
			_configFile.SetValue("graphics", "fullscreen", false);
		
		SaveSettings();
	}

	public void SaveSettings()
	{
		_configFile.Save(SettingsPath);
		
		DisplayServer.WindowSetMode(IsFullscreen
			? DisplayServer.WindowMode.Fullscreen
			: DisplayServer.WindowMode.Windowed);
		
		DisplayServer.WindowSetSize(ResolutionDict[Resolution]);
	}
}