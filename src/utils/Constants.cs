using System;
using Godot;

namespace BitBuster.utils;

public static class Constants
{
    public static float HalfPiOffset => Mathf.Pi / 2;
    public static int RoomSize => 320;
    public static int CellSize => 16;

    public static Vector2I PIT_TILE = new Vector2I(1, 3);
    public static Vector2I BREAKABLE_TILE = new Vector2I(4, 0);

}