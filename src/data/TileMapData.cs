using Godot;
using Godot.Collections;

namespace BitBuster.data;

public record TileMapData(Vector2I Offset, int Cell, int TargetId, Vector2I AtlasCoords, Vector2I Direction)
{
    
    public Vector2I Offset { get; set; } = Offset;
    public int Cell { get; set; } = Cell;
    public int TargetId { get; set; } = TargetId;
    public Vector2I Direction { get; set; } = Direction;
    public Vector2I AtlasCoords { get; set; } = AtlasCoords;
}