using System;

namespace BitBuster.weapon;

[Flags]
public enum TrajectoryType
{
    Wave = 1,
    Orbital = 2,
    Cursor = 4,
}