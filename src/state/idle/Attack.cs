using Godot;
using System;
using BitBuster.entity.enemy;
using BitBuster.state;
using BitBuster.utils;

public partial class Attack : State
{
	
	private Enemy _parent;
	private VisibleOnScreenNotifier2D _notifier;
	
	private RandomNumberGenerator _randomNumberGenerator;

	public override void Init()
	{
		_parent = GetParent().GetParent<CharacterBody2D>() as Enemy;
		_notifier = GetParent().GetParent().GetNode<VisibleOnScreenNotifier2D>("VisibleOnScreenNotifier2D");

		_randomNumberGenerator = new RandomNumberGenerator();
		_randomNumberGenerator.Randomize();
	}

	public override void Enter()
	{
		Logger.Log.Information(_parent.Name + " entering scan.");
	}

	public override void Exit()
	{
	}

	public override void StateUpdate()
	{
	}

	public override void StatePhysicsUpdate()
	{
		if (!_notifier.IsOnScreen())
		{
			EmitSignal(SignalName.StateTransition, this, "sleep");
		}

		_parent.SetGunRotationAndPosition(Mathf.Pi/12);

		if (_parent.CanSeePlayer() && _randomNumberGenerator.Randf() > 0.3f)
			_parent.WeaponComponent.AttemptShoot(_parent.Player.Position.AngleToPoint(_parent.Position) + _randomNumberGenerator.RandfRange(-Mathf.Pi / 9, Mathf.Pi / 9));

	}
}
