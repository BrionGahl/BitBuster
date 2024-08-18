using System.Collections.Generic;
using BitBuster.weapon;
using BitBuster.world;
using Godot;

namespace BitBuster.data;

public record AttackData(
    float Damage,
    EffectType Effects,
    SourceType SourceType,
    bool IsCrit)
{
    public float Damage { get; set; } = Damage;
    public EffectType Effects { get; set; } = Effects;
    public SourceType SourceType { get; set; } = SourceType;
    public bool IsCrit { get; set; } = IsCrit;
}