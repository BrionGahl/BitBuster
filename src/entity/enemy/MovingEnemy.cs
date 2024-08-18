using Godot;

namespace BitBuster.entity.enemy;

public abstract partial class MovingEnemy: Enemy
{
	public NavigationAgent2D Agent;
	protected Timer AgentTimer;
	
	private int _movementScalar;
	private float _rotationGoal;
	
	public override void _Ready()
	{
		base._Ready();

		Agent = GetNode<NavigationAgent2D>("Agent");
		AgentTimer = GetNode<Timer>("Agent/Timer");
	}

	public void MoveAction(double delta)
	{
		Vector2 goalVector = (Agent.GetNextPathPosition() - GlobalPosition).Normalized();
		
		if (goalVector == Vector2.Zero)
			return;

		Vector2 rotationVector = Vector2.FromAngle(Rotation).Normalized();
		if (rotationVector.DistanceTo(-goalVector.Normalized()) < 0.1f)
		{
			_movementScalar = 1;
			_rotationGoal -= 2 * Mathf.Pi;
		} else if (rotationVector.DistanceTo(goalVector.Normalized()) > 0.6f)
		{
			_movementScalar = 0;
			_rotationGoal = goalVector.Angle();
		}
		else
		{ 
			_movementScalar = 1;
			_rotationGoal = goalVector.Angle();
		}
		
		Rotation = Mathf.RotateToward(rotationVector.Angle(), _rotationGoal, 0.05f);
		
		Velocity = goalVector.Normalized() * _movementScalar * Speed;
		MoveAndSlide();
	}
	protected abstract void OnAgentTimeout();
}
