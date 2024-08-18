using Godot;

namespace BitBuster.component;

public partial class TrackTrailComponent : Node2D
{
	private Marker2D _leftTrackStart;
	private Marker2D _rightTrackStart;

	private LineTrailComponent _leftTrail;
	private LineTrailComponent _rightTrail;
	
	public override void _Ready()
	{
		_leftTrackStart = GetNode<Marker2D>("LeftTrackStart");
		_rightTrackStart = GetNode<Marker2D>("RightTrackStart");

		_leftTrail = GetNode<LineTrailComponent>("LeftTrackStart/TrailComponent");
		_leftTrail.Parent = _leftTrackStart;
		
		_rightTrail = GetNode<LineTrailComponent>("RightTrackStart/TrailComponent");
		_rightTrail.Parent = _rightTrackStart;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
