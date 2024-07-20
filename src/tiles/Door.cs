using System;
using BitBuster.utils;
using BitBuster.world;
using Godot;

namespace BitBuster.tiles;

public enum DoorType
{
	Left = 0,
	Top = 90,
	Right = 180,
	Bottom = 270
}

public partial class Door : Area2D
{
	public DoorType Type { get; set; }
	public Vector2I Offset { get; set; }
	
	private GlobalEvents _globalEvents;
	
	private Sprite2D _doorFrame;
	private Sprite2D _doorClosed;
	private StaticBody2D _entityBlockingBody;
	private CollisionShape2D _collisionShape;
	private GpuParticles2D _openDoorEmitter;
	private Marker2D _destination;
	
	public override void _Notification(int what)
	{
		if (what != NotificationSceneInstantiated) 
			return;
		
		_collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
		_doorFrame = GetNode<Sprite2D>("DoorFrame");
		_doorClosed = GetNode<Sprite2D>("DoorClosed");
		_openDoorEmitter = GetNode<GpuParticles2D>("OpenDoorEmitter");
		_entityBlockingBody = GetNode<StaticBody2D>("EntityBlockingBody");
		_destination = GetNode<Marker2D>("Destination");
		
		BodyEntered += OnBodyEntered;
	}

	public void SetDoorInfo(float radian, Vector2I position, Vector2I offset)
	{
		Type = (DoorType)(int)((radian * 180 / Math.PI) % 360);
		Offset = offset;

		Rotation = radian;
		Position = position;

		switch (Type)
		{
			case (DoorType.Left):
				Position = position;
				break;
			case (DoorType.Top):
				Position = position + Vector2I.Right * 32;
				break;
			case (DoorType.Right):
				Position = position + Vector2I.Right * 16 + Vector2I.Down * 32;
				break;
			case (DoorType.Bottom):
				Position = position + Vector2I.Down * 16;
				break;
		}
	}

	public void FinalizeDoor()
	{
		// NOTE: This is needed since we can't grab the GlobalEvents singleton from a non-parented node. 
		_globalEvents = GetNode<GlobalEvents>("/root/GlobalEvents");
		_globalEvents.ToggleDoors += OnToggleDoors;
	}

	private void OnBodyEntered(Node2D body)
	{
		if (!body.IsInGroup("player"))
			return; 
		
		Logger.Log.Information("body entered: {@Type}", Type);

		body.GlobalPosition = _destination.GlobalPosition + Offset;
	}

	private void OnToggleDoors(bool isOpen)
	{
		_doorClosed.Visible = !isOpen;
		_openDoorEmitter.Emitting = isOpen;
		_entityBlockingBody.SetCollisionLayerValue((int)BBCollisionLayer.World, !isOpen);;
	}
}
