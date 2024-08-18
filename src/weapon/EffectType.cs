using System;

namespace BitBuster.weapon;

[Flags]
public enum EffectType
{
    Fire = 1,
    Confuse = 2,
    Freeze = 4,
}