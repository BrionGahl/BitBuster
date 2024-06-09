using Godot;
using Godot.Collections;

namespace BitBuster.data;

public class TileMapData
{
    // SPECIAL TILES
    public static Vector2I BREAKABLE_TILE = new Vector2I(4, 0);
    
    public Vector2I Offset { get; set; }
    public int Cell { get; set; }
    public int TargetId { get; set; }
    public Vector2I Direction { get; set; }
    public Vector2I AtlasCoords { get; set; }
    
    public TileMapData(Vector2I offset, int cell, int targetId, Vector2I atlasCoords, Vector2I direction)
    {
        Offset = offset;
        Cell = cell;
        TargetId = targetId;
        AtlasCoords = atlasCoords;
        Direction = direction;
    }
}