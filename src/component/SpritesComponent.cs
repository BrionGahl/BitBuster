using BitBuster.utils;
using Godot;

namespace BitBuster.component;

public partial class SpritesComponent : Node2D
{
	
	[Export] 
	public Node2D Gun { get; private set; }
	
	[Export] 
	public Node2D Body { get; private set; }

	public override void _Ready()
	{
		Gun = GetNode<Node2D>("GunSprite").GetChildOrNull<Node2D>(0);
		Body = GetNode<Node2D>("BodySprite").GetChildOrNull<Node2D>(0);
	}

	public void SetGunRotationAndPosition(bool canSeePlayer, Vector2 playerPosition, float radian = 0)
	{
		if (Gun == null)
			return;
		
		if (canSeePlayer)
			Gun.Rotation = (float)Mathf.LerpAngle(Gun.Rotation, playerPosition.AngleToPoint(GetParent<Node2D>().Position) - Constants.HalfPiOffset, 0.5);
		else
			Gun.Rotation = (float)Mathf.LerpAngle(Gun.Rotation, Gun.Rotation + radian, 0.1);
		Gun.Position = GetParent<Node2D>().Position;
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
