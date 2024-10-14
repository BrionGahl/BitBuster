using BitBuster.utils;
using Godot;
using Godot.Collections;

namespace BitBuster.menu.other;

public partial class RootPauseMenu : Control
{
	private bool _isPaused;
	private Dictionary<string, Menu> _menus;

	
	public override void _Ready()
	{
		_menus = new Dictionary<string, Menu>();
		
		foreach (var child in GetChildren())
		{
			if (child is Menu menu)
			{
				Logger.Log.Information("Loaded {@Name}...", menu.Name.ToString());
				_menus.Add(child.Name.ToString().ToLower(), menu);
				menu.MenuNavigation += OnMenuNavigation;
			}
		}

		((BaseMenu)_menus["basemenu"]).TogglePause += OnTogglePause;
		
		Logger.Log.Information("RootMenu is Ready!");
	}
	
	public override void _UnhandledInput(InputEvent @event) 
	{
		if (@event.IsActionPressed("pause"))
			OnTogglePause();
	}

	private void OnTogglePause()
	{
		_isPaused = !_isPaused;
			
		GetTree().Paused = _isPaused;
		Visible = _isPaused;

		_menus["basemenu"].Visible = true;
		_menus["optionmenu"].Visible = false;
	}
	
	private void OnMenuNavigation(Menu menu, string destination)
	{
		menu.Visible = false;
		_menus[destination].Visible = true;
	}
	
}
