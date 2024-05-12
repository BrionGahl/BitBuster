using System;
using System.Collections.Generic;
using BitBuster.data;
using Godot;

namespace BitBuster.component;

// https://docs.godotengine.org/en/stable/tutorials/scripting/c_sharp/c_sharp_exports.html
// See this for bitwise Enums

[Flags]
public enum EffectType
{
	Normal = 0,
	Homing = 1
}

public enum WeaponType
{
	Normal = 0,
	Tri = 1,
	Laser = 4,
}

public enum SourceType
{
	Player = 2,
	Enemy = 3
}

public partial class StatsComponent : Node2D
{

	// Misc Related Stats
	[Export]
	public SourceType Source { get; set; }
	
	// Health Related Stats
	[Export]
	public float MaxHealth { get; set; }
	[Export]
	public float CurrentHealth { get; set; }

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
}
