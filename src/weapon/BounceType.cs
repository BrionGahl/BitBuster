using System;

namespace BitBuster.weapon;

[Flags]
public enum BounceType
{
    Compounding = 1,
    Splitting = 2,
    Accelerating = 4,
    Changing = 8,
    
}