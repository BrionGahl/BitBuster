using System.Collections.Generic;
using BitBuster.world;
using Godot;

namespace BitBuster.data;

public class AttackData
{
    public float Damage { get; set; }
    public float Speed { get; set; }
    public int Bounces { get; set; }
    public Vector2 Size { get; set; }
    public EffectType Effects { get; set; }
    public SourceType SourceType { get; set; }


    public AttackData(float damage, float speed, int bounces, Vector2 size, EffectType effect, SourceType sourceType)
    {
        Damage = damage;
        Effects = effect;
        Speed = speed;
        Size = size;
        Bounces = bounces;
        SourceType = sourceType;
    }
}