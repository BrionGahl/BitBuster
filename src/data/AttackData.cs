using System.Collections.Generic;

namespace BitBuster.data;

public enum EffectType
{
    Normal = 0,
    Fire = 1,
    Poison = 2,
    Electric = 3
}

public class AttackData
{
    public float Damage { get; set; }
    public List<EffectType> Effects { get; set; }


    public AttackData(float damage, List<EffectType> effects)
    {
        Damage = damage;
        Effects = effects;
    }
}