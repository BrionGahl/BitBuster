using System.Collections.Generic;
using BitBuster.world;

namespace BitBuster.data;

public class AttackData
{
    public float Damage { get; set; }
    public float Speed { get; set; }
    public int Bounces { get; set; }
    public EffectType Effects { get; set; }
    public SourceType SourceType { get; set; }


    public AttackData(float damage, float speed, int bounces, EffectType effect, SourceType sourceType)
    {
        Damage = damage;
        Effects = effect;
        Speed = speed;
        Bounces = bounces;
        SourceType = sourceType;
    }
}