using System.Collections.Generic;
using BitBuster.component;

namespace BitBuster.data;

public class AttackData
{
    public float Damage { get; set; }
    public float Speed { get; set; }
    public int Bounces { get; set; }
    public EffectType Effects { get; set; }


    public AttackData(float damage, float speed, int bounces, EffectType effect)
    {
        Damage = damage;
        Effects = effect;
        Speed = speed;
        Bounces = bounces;
    }
}