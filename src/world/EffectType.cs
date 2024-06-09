using System;

namespace BitBuster.world;

[Flags]
public enum EffectType
{
    Normal = 0,
    Fire = 1,
    Confuse = 2,
    Freeze = 4,
}