using System;
using System.Collections.Generic;
using BitBuster.component;
using BitBuster.entity.player;
using BitBuster.resource;
using BitBuster.state;
using BitBuster.utils;
using BitBuster.weapon;
using BitBuster.world;
using Godot;
using Godot.Collections;

namespace BitBuster.entity.enemy;

public abstract partial class Enemy: Entity
{
	[Export]
	private DropTable _dropTable;

	protected float Speed
	{
		get => EntityStats.Speed;
		private set => EntityStats.Speed = value;
	}

	private bool IsIdle => Velocity.Equals(Vector2.Zero);

	public Player Player;
	
	protected Global Global { get; private set; }
	protected GlobalEvents GlobalEvents { get; private set; }

	protected RandomNumberGenerator RandomNumberGenerator { get; private set; }
	
	public HealthComponent HealthComponent { get; private set; }
	protected HitboxComponent HitboxComponent { get; private set; }
	protected WeaponComponent WeaponComponent { get; private set; }
	protected SpritesComponent SpritesComponent { get; private set; }

	public VisibleOnScreenNotifier2D Notifier { get; private set; }
	private AnimationPlayer AnimationPlayer { get; set; }
	
	public Vector2 SpawnPosition { get; set; }
	public Vector2 Target { get; set; }
	
	
	protected bool HasDied;
	protected bool AnimationFinished;
	
	 public override void _Ready()
	 {
		 base._Ready();
		 
		 Player = GetTree().GetFirstNodeInGroup("player") as Player;

		 Global = GetNode<Global>("/root/Global");
		 GlobalEvents = GetNode<GlobalEvents>("/root/GlobalEvents");

		 HealthComponent = GetNode<HealthComponent>("HealthComponent");
		 HitboxComponent = GetNode<HitboxComponent>("HitboxComponent");
		 SpritesComponent = GetNode<SpritesComponent>("SpritesComponent"); 
		 WeaponComponent = GetNodeOrNull<WeaponComponent>("WeaponComponent");
		 
		 if (WeaponComponent != null)
			WeaponComponent.EntityStats = EntityStats;
		 
		 HealthComponent.EntityStats = EntityStats; 
		 HitboxComponent.HealthComponent = HealthComponent;

		 Notifier = GetNode<VisibleOnScreenNotifier2D>("VisibleOnScreenNotifier2D");
		 AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		 
		 RandomNumberGenerator = new RandomNumberGenerator();
		 RandomNumberGenerator.Randomize();
		 
		 SpawnPosition = Position;
		 
		 HealthComponent.HealthIsZero += OnHealthIsZero;
		 HealthComponent.HealthChange += OnHealthChange;
	 }

	public bool CanSeePlayer(int bounces = 0)
	{
		PhysicsDirectSpaceState2D spaceState = GetWorld2D().DirectSpaceState;
		PhysicsRayQueryParameters2D query;
		query = PhysicsRayQueryParameters2D.Create(Position, Player.Position, 0b100111, new Array<Rid> { GetRid() });
		Dictionary results = spaceState.IntersectRay(query);
		if (results.Count == 0)
			return false;
		return results["rid"].AsRid() == Player.GetRid();
	}

	public void MakeElite(EliteType type)
	{
		Logger.Log.Information("Spawning Elite of {@type}.", type);
		switch (type)
		{
			case EliteType.Tough:
				EntityStats.MaxHealth *= 1.5f;
				SetColor(Colors.Orange);
				break;
			case EliteType.Quick:
				EntityStats.Speed *= 1.25f;
				SetColor(Colors.Yellow);
				break;
			case EliteType.Deadly:
				EntityStats.ProjectileDamage *= 2f;
				SetColor(Colors.Red);
				break;
			case EliteType.Invisible:
				SetColor(new Color(Colors.White, 0.5f));
				break;
			case EliteType.Chaotic:
				EntityStats.ProjectileWeaponType |= WeaponType.Random;
				SetColor(Colors.LightBlue);
				break;
		}
	}

	public void HandleAnimations()
	{
		SpritesComponent.PlayAnimation(IsIdle);
	}

	protected bool AttemptToFree()
	{
		if (!HasDied)
			return false;

		if (WeaponComponent is { BombsChildren: > 0 } || WeaponComponent is { BulletsChildren: > 0 } || !AnimationFinished)
			return false;

		Logger.Log.Information(Name + " freed.");
		QueueFree();
		return true;
	}

	protected void HandleDrops()
	{
		foreach (Drop drop in _dropTable.DropsList)
		{
			var chance = RandomNumberGenerator.Randf();
			if (chance > drop.Chance)
				continue;
			
			var amount = RandomNumberGenerator.RandiRange(1, drop.MaxAmount);
			for (int i = 0; i < amount; i++)
				GlobalEvents.EmitSpawnItemEventHandler(Position, (int)drop.ItemType, drop.ItemId);
		}	
	}

	private void SetColor(Color color)
	{
		if (SpritesComponent.Gun != null)
			SpritesComponent.Gun.Modulate = color;
		if (SpritesComponent.Body != null)
			SpritesComponent.Body.Modulate = color;
	}
	
	private void OnHealthChange(float value)
	{
		AnimationPlayer.Play("effect_damage_blink", -1D, EntityStats.ITime / 0.2f);
	}
	
	public abstract void AttackAction(double delta);
	protected abstract void OnHealthIsZero();
}
