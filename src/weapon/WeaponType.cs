using System;

namespace BitBuster.weapon;

[Flags]
public enum WeaponType
{
    Bi = 1,
    Tri = 2,
    Quad = 4 | 1,
    Random = 8,
    
}