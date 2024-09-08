using BitBuster.utils;
using BitBuster.world;
using Godot;

namespace BitBuster.menu;

public partial class Menu : MarginContainer
{
    [Signal]
    public delegate void MenuNavigationEventHandler(Menu menu, string destination);

    protected GlobalButtonSFX GlobalButtonSfx;

    public override void _Ready()
    {
        GlobalButtonSfx = GetNode<GlobalButtonSFX>("/root/GlobalButtonSfx");
    }
}