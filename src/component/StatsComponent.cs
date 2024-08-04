using System;
using System.Collections.Generic;
using BitBuster.data;
using BitBuster.items;
using BitBuster.world;
using Godot;

namespace BitBuster.component;

// https://docs.godotengine.org/en/stable/tutorials/scripting/c_sharp/c_sharp_exports.html
// See this for bitwise Enums

public partial class StatsComponent : Node2D
{
	[Signal]
	public delegate void StatChangeEventHandler();
	
	[Signal]
	public delegate void ItemAddedEventHandler(Item item);
	
	// Misc Related Stats
	[Export]
	public SourceType Source { get; set; }
	[Export] 
	public int BombCount { get; set; }
	[Export]
	public int KeyCardCount { get; set; }
	[Export]
	public int CreditCount { get; set; }
	
	// Health Related Stats
	[Export]
	public float MaxHealth { get; set; }
	[Export]
	public float CurrentHealth { get; set; }
	[Export]
	public float Overheal { get; set; } // NYI

	// Bullet Related Stats
	[Export]
	public float ProjectileDamage { get; set; }
	[Export]
	public int ProjectileCount { get; set; }
	[Export]
	public float ProjectileCooldown { get; set; }
	[Export]
	public int ProjectileBounces { get; set; }
	[Export]
	public float ProjectileSpeed { get; set; }
	[Export]
	public Vector2 ProjectileSizeScalar { get; set; }
	[Export]
	public EffectType ProjectileDamageType { get; set; }
	[Export]
	public WeaponType ProjectileWeaponType { get; set; }
	[Export]
	public BulletType ProjectileBulletType { get; set; }
	[Export]
	public float ProjectileAccuracy { get; set; }
	
	// Bomb Related Stats
	[Export]
	public float BombDamage { get; set; }
	[Export]
	public EffectType BombDamageType { get; set; }
	[Export]
	public float BombRadius { get; set; }
	
	// Control Related Stats
	[Export]
	public float Speed { get; set; }
	[Export]
	public float ITime { get; set; }
	[Export]
	public EffectType TrailEffect { get; set; }
	
	// Other Stats
	[Export]
	public int Luck { get; private set; }
	[Export]
	public bool OverhealBurst { get; private set; }
	[Export]
	public float OverhealRegen { get; private set; }

	
	private RandomNumberGenerator _random;

	public override void _Ready()
	{
		_random = new RandomNumberGenerator();
	}

	public AttackData GetAttackData()
	{
		float critChance = _random.Randf();
		if (critChance > 1.0f - Luck/10f)
			return new AttackData(ProjectileDamage * 1.5f, ProjectileDamageType, Source, true);
		return new AttackData(ProjectileDamage, ProjectileDamageType, Source, false);
	}

	public AttackData GetBombAttackData()
	{
		float critChance = _random.Randf();
		if (critChance > 1.0f - Luck/10f)
			return new AttackData(BombDamage * 1.5f, BombDamageType, Source, true);
		return new AttackData(BombDamage, BombDamageType, Source, false);
	}

	public void AddItem(Item item)
	{
		EmitSignal(SignalName.ItemAdded, item);

		MaxHealth = MaxHealth + item.MaxHealth < 1
			? 1
			: MaxHealth + item.MaxHealth;
		
		ProjectileDamage += item.ProjectileDamage;
		ProjectileCount += item.ProjectileCount;
		ProjectileCooldown = ProjectileCooldown - item.ProjectileCooldown < 0.05f
			? 0.05f
			: ProjectileCooldown - item.ProjectileCooldown;
		ProjectileBounces += item.ProjectileBounces;
		ProjectileSpeed += item.ProjectileSpeed;
		ProjectileSizeScalar += item.ProjectileSizeScalar;
		ProjectileDamageType |= item.ProjectileDamageType;
		ProjectileWeaponType |=  item.ProjectileWeaponType;
		ProjectileBulletType |=  item.ProjectileBulletType;
		ProjectileAccuracy = ProjectileAccuracy - item.ProjectileAccuracy < 0
			? 0.0f
			: ProjectileAccuracy - item.ProjectileAccuracy;
		
		BombDamage += item.BombDamage;
		BombDamageType |= item.BombDamageType;
		BombRadius += item.BombRadius;
		
		Speed += item.Speed;
		ITime += item.ITime;
		TrailEffect |= item.TrailEffect;

		Luck += item.Luck;
		OverhealBurst = item.OverhealBurst;
		OverhealRegen += item.OverhealRegen;
	}

	public void EmitStatChangeSignal()
	{
		EmitSignal(SignalName.StatChange);
	}
}
