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
	public Vector2 ProjectileSizeScalar { get; set; }
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
	public float ITime { get; set; }
	[Export]
	public EffectType TrailEffect { get; set; }
		
	public override void _Ready()
	{
		// Logic for default stats go here...
	}

	public AttackData GetAttackData()
	{
		return new AttackData(ProjectileDamage, ProjectileSpeed, ProjectileBounces, ProjectileSizeScalar, ProjectileDamageType, Source);
	}

	public AttackData GetBombAttackData()
	{
		return new AttackData(BombDamage, 0f, 0, Vector2.Zero, EffectType.Normal, Source);
	}

	public void AddItem(Item item)
	{
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
		ProjectileDamageType = ProjectileDamageType | item.ProjectileDamageType;
		ProjectileWeaponType = ProjectileWeaponType | item.ProjectileWeaponType;

		BombDamage += item.BombDamage;
		Speed += item.Speed;
		ITime += item.ITime;
		TrailEffect = TrailEffect | item.TrailEffect;
	}
}
