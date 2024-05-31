using System;
using System.Collections.Generic;
using BitBuster.data;
using BitBuster.world;
using Godot;

namespace BitBuster.component;

// https://docs.godotengine.org/en/stable/tutorials/scripting/c_sharp/c_sharp_exports.html
// See this for bitwise Enums

public partial class StatsComponent : Node2D
{
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
	public float Overheal { get; set; }

	// Weapon Related Stats
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
	public EffectType ProjectileDamageType { get; set; }
	[Export]
	public WeaponType ProjectileWeaponType { get; set; }
	[Export]
	public float BombDamage { get; set; }
	// Control Related Stats
	[Export]
	public float Speed { get; set; }
	[Export]
	public EffectType TrailEffect { get; set; }
	[Export]
	public float SizeScalar { get; set; } // NOT IMPLEMENTED
	
	public override void _Ready()
	{
		// Logic for default stats go here...
	}

	public AttackData GetAttackData()
	{
		return new AttackData(ProjectileDamage, ProjectileSpeed, ProjectileBounces, ProjectileDamageType, Source);
	}

	public AttackData GetBombAttackData()
	{
		return new AttackData(BombDamage, 0f, 0, EffectType.Normal, Source);
	}

	public void AddItemData(ItemData itemData)
	{
		// TODO: soon... tm.
	}
}
