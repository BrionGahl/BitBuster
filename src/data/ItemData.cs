using BitBuster.component;
using BitBuster.world;
using Godot;

namespace BitBuster.data;

public class ItemData
{
    public string ItemName { get; private set; }
    public bool IsStattedItem { get; private set; }
    
    public int AddedBombs { get; private set; }
    public int AddedKeyCard { get; private set; }
    public int AddedCredit { get; private set; }
    public float AddedHealth { get; private set; }
    public float AddedOverheal { get; private set; }
    
    // Health Related Stats
    public float MaxHealth { get; private set; }

    // Weapon Related Stats
    public float ProjectileDamage { get; private set; }
    public int ProjectileCount { get; private set; }
    public float ProjectileCooldown { get; private set; }
    public int ProjectileBounces { get; private set; }
    public float ProjectileSpeed { get; private set; }
    public EffectType ProjectileDamageType { get; private set; }
    public WeaponType ProjectileWeaponType { get; private set; }
    public float BombDamage { get; private set; }
    
    // Control Related Stats
    public float Speed { get; private set; }
    public EffectType TrailEffect { get; private set; }
    public float SizeScalar { get; private set; } // NOT IMPLEMENTED

    public ItemData(string itemName, int addedBombs, int addedKeyCard, int addedCredit, float addedHealth, float addedOverheal)
    {
        IsStattedItem = false;
        ItemName = itemName;

        AddedBombs = addedBombs;
        AddedKeyCard = addedKeyCard;
        AddedCredit = addedCredit;
        AddedHealth = addedHealth;
        AddedOverheal = addedOverheal;
    }
    
    
    public ItemData(string itemName, int addedBombs=0, int addedKeyCard=0, int addedCredit=0, float addedHealth=0, float addedOverheal=0, float maxHealth=0, float projectileDamage=0, int projectileCount=0, float projectileCooldown=0, int projectileBounces=0, float projectileSpeed=0, EffectType projectileDamageType=EffectType.Normal, WeaponType projectileWeaponType=WeaponType.Normal, float bombDamage=0, float speed=0, EffectType trailEffect=EffectType.Normal, float sizeScalar=0f)
    {
        IsStattedItem = true;
        ItemName = itemName;
        
        AddedBombs = addedBombs;
        AddedKeyCard = addedKeyCard;
        AddedCredit = addedCredit;
        AddedHealth = addedHealth;
        AddedOverheal = addedOverheal;
        
        MaxHealth = maxHealth;
        ProjectileDamage = projectileDamage;
        ProjectileCount = projectileCount;
        ProjectileCooldown = projectileCooldown;
        ProjectileBounces = projectileBounces;
        ProjectileSpeed = projectileSpeed;
        ProjectileDamageType = projectileDamageType;
        ProjectileWeaponType = projectileWeaponType;
        BombDamage = bombDamage;
        Speed = speed;
        TrailEffect = trailEffect;
        SizeScalar = sizeScalar;
    }
}