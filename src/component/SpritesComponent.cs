using BitBuster.utils;
using Godot;

namespace BitBuster.component;

public partial class SpritesComponent : Node2D
{
	
	[Export] 
	public Node2D Gun { get; private set; }
	
	[Export] 
	public Node2D Body { get; private set; }

	private RemoteTransform2D _remoteTransform2D;

	public override void _Ready()
	{
		Body = GetChildOrNull<Node2D>(1);
		Gun = GetChildOrNull<Node2D>(2);

		_remoteTransform2D = GetNode<RemoteTransform2D>("RemoteTransform2D");

		if (Gun != null)
			_remoteTransform2D.RemotePath = Gun.GetPath();
	}

	public void SetGunRotation(bool canSeePlayer, Vector2 playerPosition, float radian = 0)
	{
		if (Gun == null)
			return;
		
		if (canSeePlayer)
			Gun.Rotation = (float)Mathf.RotateToward(Gun.Rotation, playerPosition.AngleToPoint(GetParent<Node2D>().Position) - Constants.HalfPiOffset, 0.5);
		else
			Gun.Rotation = (float)Mathf.RotateToward(Gun.Rotation, Gun.Rotation + radian, 0.1);
	}

	public void PlayAnimation(bool isIdle)
	{
		if (Body is not AnimatedSprite2D sprite2D) return;
		
		sprite2D.Animation = isIdle ? "default" : "moving";
		sprite2D.Play();
	}

	public void SetBodyMaterialProperty(string property, float value)
	{
		Body.Material.Set(property, value);
	}
	
}
