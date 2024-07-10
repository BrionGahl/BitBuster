using System.Collections.Generic;
using Godot;

namespace BitBuster.data;

public record RoomData
{
    public List<Node2D> Objects { get; set; } = new();
    public List<TileMapData> TileMap { get; set; } = new();
}