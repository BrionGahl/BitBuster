using BitBuster.world;
using Godot;

namespace BitBuster.tiles;

public partial class BreakableWall : StaticBody2D
{
	private GpuParticles2D _shatterEmitter;
	private CollisionShape2D _collisionShape;
	private Sprite2D _sprite;
	
	private bool _isBroken;
	
	public override void _Ready()
	{
		_shatterEmitter = GetNode<GpuParticles2D>("ShatterEmitter");
		_collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
		_sprite = GetNode<Sprite2D>("Sprite2D");

		_shatterEmitter.Finished += OnShatterFinished;
	}
	
	public void Break()
	{
		if (_isBroken)
			return;
		_isBroken = true;
		SetCollisionLayerValue((int)BbCollisionLayer.World, false);
		_sprite.Visible = false;
		
		_shatterEmitter.Emitting = true;
	}

	private void OnShatterFinished()
	{
		QueueFree();
	}
}
