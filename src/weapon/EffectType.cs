using System;

namespace BitBuster.weapon;

[Flags]
public enum EffectType
{
    Fire = 1,
    Oil = 2,
    Water = 4,
    Poison = 8
}