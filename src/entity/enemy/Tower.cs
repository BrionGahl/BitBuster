using BitBuster.utils;
using Godot;

namespace BitBuster.entity.enemy;

public partial class Tower : Enemy
{
	
	private Sprite2D _gun;
	private Sprite2D _body;
	

	public override void _Ready()
	{
		base._Ready();
		
		_gun = GetNode<Sprite2D>("Gun");
		_body = GetNode<Sprite2D>("Body");
		
	}
	
	public override void SetGunRotationAndPosition(float radian = 0)
	{
		if (CanSeePlayer())
			_gun.Rotation = (float)Mathf.LerpAngle(_gun.Rotation, Player.Position.AngleToPoint(Position) - Constants.HalfPiOffset, 0.5);
		else
			_gun.Rotation = (float)Mathf.LerpAngle(_gun.Rotation, _gun.Rotation + radian, 0.1);
		_gun.Position = Position;
	}	

	public override void HandleAnimations()
	{
	}
}
