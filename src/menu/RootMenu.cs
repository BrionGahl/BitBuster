using BitBuster.utils;
using BitBuster.world;
using Godot;
using Godot.Collections;

namespace BitBuster.menu;

public partial class RootMenu : Control
{
	
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
		Logger.Log.Information("RootMenu is Ready!");
	}

	private void OnMenuNavigation(Menu menu, string destination)
	{
		menu.Visible = false;
		_menus[destination].Visible = true;
	}
}
