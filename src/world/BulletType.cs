using System;

namespace BitBuster.world;

[Flags]
public enum BulletType
{
    Piercing = 1,
    Exploding = 2,
    Invulnerable = 4
}