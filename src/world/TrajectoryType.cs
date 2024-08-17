using System;

namespace BitBuster.world;

[Flags]
public enum TrajectoryType
{
    Wave = 1,
    Orbital = 2,
    Cursor = 4,
}