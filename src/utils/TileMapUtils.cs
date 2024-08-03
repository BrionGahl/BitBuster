using System.Collections.Generic;
using Godot;

namespace BitBuster.utils;

public static class TileMapUtils
{
    public static List<Vector2> GetTilesInRadius(Vector2 pos, float r, TileMap tileMap)
    {
        List<Vector2> tilesFound = new List<Vector2>();
        
        foreach (Vector2I cellPos in tileMap.GetUsedCells(0))
        {
            Vector2 wPos = tileMap.MapToLocal(cellPos);
            if (pos.DistanceTo(wPos) < r)
                tilesFound.Add(wPos);
        }
        
        return tilesFound;
    }
}