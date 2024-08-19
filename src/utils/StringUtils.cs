using Godot;

namespace BitBuster.utils;

public static class StringUtils
{
    public static Vector2I StringToVector2I(string s)
    {
        Logger.Log.Information(s);
        string[] coords = s.Split("x");
        return new Vector2I(coords[0].ToInt(), coords[1].ToInt());
    }
}