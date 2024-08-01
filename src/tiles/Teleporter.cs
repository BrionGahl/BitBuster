using BitBuster.utils;
using BitBuster.world;
using Godot;

namespace BitBuster.tiles;

public partial class Teleporter : Area2D
{
	private GlobalEvents _globalEvents;

	private Sprite2D _teleporterActiveSprite;
	private GpuParticles2D _teleporterActiveEmitter;

	private bool _active;

	public override void _Ready()
	{
		_globalEvents = GetNode<GlobalEvents>("/root/GlobalEvents");

		_teleporterActiveSprite = GetNode<Sprite2D>("TeleporterActive");
		_teleporterActiveEmitter = GetNode<GpuParticles2D>("TeleporterActiveEmitter");

		_active = true;
		BodyEntered += OnBodyEntered;
		_globalEvents.ToggleDoors += OnToggleDoors;
	}

	private void OnBodyEntered(Node2D body)
	{
		if (!body.IsInGroup(Groups.GroupPlayer))
			return;
		
		Logger.Log.Information("Player entered teleporter, going to the next floor...");
		body.Position = Constants.SPAWN_POSITION;
		_globalEvents.CallDeferred("EmitIncrementAndGenerateLevelSignal");
		_active = false;
	}

	private void OnToggleDoors(bool isOpen)
	{
		if (!_active)
			return;
		
		_teleporterActiveEmitter.Emitting = isOpen;
		_teleporterActiveSprite.Visible = isOpen;
		SetDeferred("monitoring", isOpen);
	}
}
