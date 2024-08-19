using BitBuster.utils;
using Godot;

namespace BitBuster.menu;

public partial class Menu : MarginContainer
{
    [Signal]
    public delegate void MenuNavigationEventHandler(Menu menu, string destination);
}