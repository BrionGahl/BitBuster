using System;

namespace BitBuster.world;

[Flags]
public enum WeaponType
{
    Bi = 1,
    Tri = 2,
    Quad = 4 | 1,
    Random = 8,
    
}