using Godot;

namespace BitBuster.tiles;

public partial class BreakableWall : StaticBody2D
{
	private NavigationRegion2D _navRegion;
	private TileMap _tileMap;
	private GpuParticles2D _shatterEmitter;
	private Timer _animationTimer;
	
	public override void _Ready()
	{
		_navRegion = GetNode<NavigationRegion2D>("/root/Floor/Level/NavRegion");
		_tileMap = GetNode<TileMap>("/root/Floor/Level/NavRegion/TileMapMain");
		_shatterEmitter = GetNode<GpuParticles2D>("ShatterEmitter");
		_animationTimer = GetNode<Timer>("AnimationTimer");

		_animationTimer.Timeout += OnAnimationTimeout;
	}
	
	public void Break()
	{
		_tileMap.SetCell(0, _tileMap.LocalToMap(GlobalPosition));
		_navRegion.BakeNavigationPolygon();

		_shatterEmitter.Emitting = true;
		_animationTimer.Start();
	}

	private void OnAnimationTimeout()
	{
		QueueFree();
	}
}
