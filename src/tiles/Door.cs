using System;
using BitBuster.utils;
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
	
	private Sprite2D _sprite2D;
	private CollisionShape2D _collisionShape;
	private Marker2D _marker2D;
	
	public override void _Notification(int what)
	{
		if (what != NotificationSceneInstantiated) 
			return;
		
		_collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
		_sprite2D = GetNode<Sprite2D>("DoorOpen");
		_marker2D = GetNode<Marker2D>("Destination");
		
		this.BodyEntered += OnBodyEntered;
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

	private void OnBodyEntered(Node2D body)
	{
		if (!body.IsInGroup("player"))
			return; 
		
		Logger.Log.Information("body entered..." + Type);

		body.GlobalPosition = _marker2D.GlobalPosition + Offset;
	}
}
