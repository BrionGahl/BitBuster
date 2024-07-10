using System.Collections.Generic;
using BitBuster.world;
using Godot;

namespace BitBuster.data;

public record AttackData(
    float Damage,
    float Speed,
    int Bounces,
    Vector2 Size,
    EffectType Effects,
    SourceType SourceType)
{
    public float Damage { get; set; } = Damage;
    public float Speed { get; set; } = Speed;
    public int Bounces { get; set; } = Bounces;
    public Vector2 Size { get; set; } = Size;
    public EffectType Effects { get; set; } = Effects;
    public SourceType SourceType { get; set; } = SourceType;
}