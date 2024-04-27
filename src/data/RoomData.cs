using System.Collections.Generic;
using Godot;

namespace BitBuster.data;

public class RoomData
{
    public List<Node2D> Objects { get; set; }
    public List<TileMapData> TileMap { get; set; }

    public RoomData()
    {
        Objects = new List<Node2D>();
        TileMap = new List<TileMapData>();
    }
}