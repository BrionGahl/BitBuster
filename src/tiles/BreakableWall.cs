using Godot;

namespace BitBuster.tiles;

public partial class BreakableWall : Area2D
{
	private NavigationRegion2D _navRegion;
	private TileMap _tileMap;

	public override void _Ready()
	{
		_navRegion = GetNode<NavigationRegion2D>("/root/Floor/Level/NavRegion");
		_tileMap = GetNode<TileMap>("/root/Floor/Level/NavRegion/TileMapMain");
	}
	
	public void Break()
	{
		_tileMap.SetCell(0, _tileMap.LocalToMap(GlobalPosition));
		_navRegion.BakeNavigationPolygon();
		QueueFree();
	}
}
