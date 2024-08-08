using System;

namespace BitBuster.world;

[Flags]
public enum BounceType
{
    Compounding = 1,
    Splitting = 2,
    Accelerating = 4,
}