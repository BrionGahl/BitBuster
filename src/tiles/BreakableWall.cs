using Godot;

namespace BitBuster.tiles;

public partial class BreakableWall : Area2D
{

	private TileMap _tileMap;

	public override void _Ready()
	{
		_tileMap = GetNode<TileMap>("/root/Floor/Level/TileMapMain");
	}
	
	public void Break()
	{
		// _tileMap.SetCell(0, _tileMap.LocalToMap(GlobalPosition));
		
		_tileMap.SetCell(0, _tileMap.LocalToMap(GlobalPosition), 0, new Vector2I(7 ,4));
		QueueFree();
	}
}
