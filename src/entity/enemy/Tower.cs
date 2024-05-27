using BitBuster.utils;
using Godot;

namespace BitBuster.entity.enemy;

public partial class Tower : Enemy
{
	
	private Sprite2D _gun;
	private Sprite2D _body;
	private CollisionShape2D _collider;
	private GpuParticles2D _particleDeath;

	private bool _hasDied;
	private bool _animationFinished;

	public override void _Ready()
	{
		base._Ready();
		
		_collider = GetNode<CollisionShape2D>("Collider");
		_gun = GetNode<Sprite2D>("Gun");
		_body = GetNode<Sprite2D>("Body");
		_particleDeath = GetNode<GpuParticles2D>("ParticleDeath");
	}
	
	public override void SetGunRotationAndPosition(float radian = 0)
	{
		if (CanSeePlayer())
			_gun.Rotation = (float)Mathf.LerpAngle(_gun.Rotation, Player.Position.AngleToPoint(Position) - Constants.HalfPiOffset, 0.5);
		else
			_gun.Rotation = (float)Mathf.LerpAngle(_gun.Rotation, _gun.Rotation + radian, 0.1);
		_gun.Position = Position;
	}

	public override void OnHealthIsZero()
	{
		_gun.Visible = false;
		_body.Visible = false;
		_collider.SetDeferred("disabled", true);
		HitboxComponent.SetDeferred("monitorable", false);
		HitboxComponent.SetDeferred("monitoring", false);

		_particleDeath.Emitting = true;

		DeathAnimationTimer.Start();
		_hasDied = true;
	}

	public override void OnDeathAnimationTimeout()
	{
		_animationFinished = true;
	}

	public override void AttackAction(double delta)
	{
		if (_hasDied)
		{
			if (WeaponComponent.GetChildCount() <= 2 && _animationFinished)
			{
				Logger.Log.Information(Name + " freed.");
				QueueFree();
			}
			return;
		}
		
		SetGunRotationAndPosition(Mathf.Pi/12);
		
		if (CanSeePlayer() && RandomNumberGenerator.Randf() > 0.3f)
			WeaponComponent.AttemptShoot(Player.Position.AngleToPoint(Position) + RandomNumberGenerator.RandfRange(-Mathf.Pi / 9, Mathf.Pi / 9));
	}
	
	public override void HandleAnimations()
	{
	}
}
