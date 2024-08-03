using BitBuster.world;
using Godot;

namespace BitBuster.tiles;

public partial class BreakableWall : StaticBody2D
{
	private NavigationRegion2D _navRegion;
	private TileMap _tileMap;
	private GpuParticles2D _shatterEmitter;
	private Timer _animationTimer;
	private CollisionShape2D _collisionShape;
	private Sprite2D _sprite;
	
	private bool _isBroken;
	
	public override void _Ready()
	{
		_shatterEmitter = GetNode<GpuParticles2D>("ShatterEmitter");
		_animationTimer = GetNode<Timer>("AnimationTimer");
		_collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
		_sprite = GetNode<Sprite2D>("Sprite2D");

		_animationTimer.Timeout += OnAnimationTimeout;
	}
	
	public void Break()
	{
		if (_isBroken)
			return;

		_isBroken = true;
		
		_sprite.Visible = false;
		_collisionShape.Disabled = true;
		
		_shatterEmitter.Emitting = true;
		_animationTimer.Start();
	}

	private void OnAnimationTimeout()
	{
		QueueFree();
	}
}
