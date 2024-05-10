using Godot;
using System;
using BitBuster.entity.enemy;
using BitBuster.utils;

public partial class Tower : Enemy
{
	
	private Sprite2D _gun;
	private Sprite2D _body;
	
	private RandomNumberGenerator _randomNumberGenerator;

	public override void _Ready()
	{
		_gun = GetNode<Sprite2D>("Gun");
		_body = GetNode<Sprite2D>("Body");
		
		_randomNumberGenerator = new RandomNumberGenerator();
		_randomNumberGenerator.Randomize();
	}
	
	
	public override void _PhysicsProcess(double delta)
	{
		
		switch (State)
		{
			case EnemyState.Idle:
				Idle();
				break;
			default:
				if (!CanSeePlayer())
					State = EnemyState.Idle;
				
				SetGunRotationAndPosition();
		
				if (CanSeePlayer() && _randomNumberGenerator.Randf() > 0.3f)
					_weaponComponent.AttemptShoot(_player.Position.AngleToPoint(Position) + _randomNumberGenerator.RandfRange(-Mathf.Pi / 9, Mathf.Pi / 9));
				break;
		}
		
	}

	private void Idle()
	{
		if (CanSeePlayer())
			State = EnemyState.Attack;
	}
	
	
	private void SetGunRotationAndPosition()
	{
		if (CanSeePlayer())
			_gun.Rotation = (float)Mathf.LerpAngle(_gun.Rotation, _player.Position.AngleToPoint(Position) - Constants.HalfPiOffset, 0.5);
		else
			_gun.Rotation = (float)Mathf.LerpAngle(_gun.Rotation, Rotation, 0.1);
		_gun.Position = Position;
	}
}
