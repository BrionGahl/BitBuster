using System;
using System.Collections.Generic;
using BitBuster.component;
using BitBuster.entity.player;
using BitBuster.state;
using BitBuster.utils;
using BitBuster.world;
using Godot;
using Godot.Collections;

namespace BitBuster.entity.enemy;

public abstract partial class Enemy: CharacterBody2D
{
	public float Speed
	{
		get => StatsComponent.Speed;
		set => StatsComponent.Speed = value;
	}

	protected float RotationSpeed => Speed / 25;
	protected bool IsIdle => Velocity.Equals(Vector2.Zero);

	public Player Player;

	protected StatsComponent StatsComponent { get; private set; }
	protected HealthComponent HealthComponent { get; private set; }
	protected HitboxComponent HitboxComponent { get; private set; }
	protected WeaponComponent WeaponComponent { get; private set; }
	protected SpritesComponent SpritesComponent { get; private set; }

	public VisibleOnScreenNotifier2D Notifier { get; private set; }
	protected Timer DeathAnimationTimer { get; private set; }
	private AnimationPlayer AnimationPlayer { get; set; }
	
	public Vector2 SpawnPosition { get; set; }
	public Vector2 Target { get; set; }
	
	protected RandomNumberGenerator RandomNumberGenerator;
	
	 public override void _Ready()
	 {
		 Player = GetTree().GetFirstNodeInGroup("player") as Player;
		 
		 StatsComponent = GetNodeOrNull<Node2D>("StatsComponent") as StatsComponent;
		 HealthComponent = GetNodeOrNull<Node2D>("HealthComponent") as HealthComponent;
		 HitboxComponent = GetNodeOrNull<Node2D>("HitboxComponent") as HitboxComponent;
		 WeaponComponent = GetNodeOrNull<Node2D>("WeaponComponent") as WeaponComponent;
		 
		 SpritesComponent = GetNode<SpritesComponent>("SpritesComponent");

		 HealthComponent.StatsComponent = StatsComponent;
		 HitboxComponent.HealthComponent = HealthComponent;
		 
		 if (WeaponComponent != null)
			WeaponComponent.StatsComponent = StatsComponent;

		 Notifier = GetNode<VisibleOnScreenNotifier2D>("VisibleOnScreenNotifier2D");
		 DeathAnimationTimer = GetNode<Timer>("DeathAnimationTimer");
		 AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		 
		 RandomNumberGenerator = new RandomNumberGenerator();
		 RandomNumberGenerator.Randomize();
		 
		 SpawnPosition = Position;

		 DeathAnimationTimer.Timeout += OnDeathAnimationTimeout;
		 HealthComponent.HealthIsZero += OnHealthIsZero;
		 HealthComponent.HealthChange += OnHealthChange;
	 }

	public bool CanSeePlayer(int bounces = 0)
	{
		PhysicsDirectSpaceState2D spaceState = GetWorld2D().DirectSpaceState;
		PhysicsRayQueryParameters2D query;
		query = PhysicsRayQueryParameters2D.Create(Position, Player.Position, 47, new Array<Rid> { GetRid() });
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
				StatsComponent.MaxHealth *= 1.5f;
				SetColor(Colors.Orange);
				break;
			case EliteType.Quick:
				StatsComponent.Speed *= 1.25f;
				SetColor(Colors.Yellow);
				break;
			case EliteType.Deadly:
				StatsComponent.ProjectileDamage *= 2f;
				SetColor(Colors.Red);
				break;
			case EliteType.Invisible:
				SetColor(Colors.Transparent);
				break;
			case EliteType.Chaotic:
				StatsComponent.ProjectileWeaponType |= WeaponType.Random;
				SetColor(Colors.LightBlue);
				break;
		}
	}

	public void HandleAnimations()
	{
		SpritesComponent.PlayAnimation(IsIdle);
	}
	
	private void OnHealthChange(float value)
	{
		AnimationPlayer.Play("effect_damage_blink", -1D, StatsComponent.ITime / 0.2f);
	}

	private void SetColor(Color color)
	{
		SpritesComponent.Gun.Modulate = color;
		SpritesComponent.Body.Modulate = color;
	}
	
	protected abstract void OnHealthIsZero();
	protected abstract void OnDeathAnimationTimeout();
	
	public abstract void AttackAction(double delta);
}
