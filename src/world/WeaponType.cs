using System;

namespace BitBuster.world;

[Flags]
public enum WeaponType
{
    Normal = 0,
    Tri = 1,
    Laser = 2,
    Bouncing = 4,
}