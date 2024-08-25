using System;

namespace BitBuster.weapon;

[Flags]
public enum EffectType
{
    Fire = 1, // DoT
    Oil = 2, // Speed Up / Slide
    Water = 4, // No Effect By Itself
    Electric = 8, // Stuns for 0.5s 
    Smoke = 1 | 4, // No See
    Explode = 1 | 2, // A Second Boom
    Sludge = 2 | 4, // Speed Significantly Down
    Electified = 4 | 8 // Stuns for 2s
    
}