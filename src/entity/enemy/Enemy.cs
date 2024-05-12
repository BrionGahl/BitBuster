using System;
using System.Collections.Generic;
using BitBuster.component;
using BitBuster.entity.player;
using BitBuster.state;
using BitBuster.utils;
using Godot;
using Godot.Collections;

namespace BitBuster.entity.enemy;

public enum EnemyState
{
    Idle = 0,
    Pursue = 1,
    Evade = 2,
    Attack = 3
}


public abstract partial class Enemy: CharacterBody2D
{
    public float Speed
    {
        get => StatsComponent.Speed;
        set => StatsComponent.Speed = value;
    }
    public float RotationSpeed => Speed / 25;
    public bool IsIdle => Velocity.Equals(Vector2.Zero);


    // TODO: Transition this to state machine...
    // Add visibility notifier for better performance and to add to idle
    public Player Player;
    
    public StatsComponent StatsComponent { get; private set; }
    public HealthComponent HealthComponent { get; private set; }
    public HitboxComponent HitboxComponent { get; private set; }
    public WeaponComponent WeaponComponent { get; private set; }
    
    public StateMachine StateMachine { get; set; }
    
    public Vector2 SpawnPosition { get; set; }
    public Vector2 Target { get; set; }
    
     public override void _Ready()
     {
         Player = GetTree().GetFirstNodeInGroup("player") as Player;
         
         StatsComponent = GetNodeOrNull<Node2D>("StatsComponent") as StatsComponent;
         HealthComponent = GetNodeOrNull<Node2D>("HealthComponent") as HealthComponent;
         HitboxComponent = GetNodeOrNull<Node2D>("HitboxComponent") as HitboxComponent;
         WeaponComponent = GetNodeOrNull<Node2D>("WeaponComponent") as WeaponComponent;
         
         StateMachine = GetNode<Node2D>("StateMachine") as StateMachine;
         
         HealthComponent.StatsComponent = StatsComponent;
         HitboxComponent.HealthComponent = HealthComponent;
         WeaponComponent.StatsComponent = StatsComponent;
         
         SpawnPosition = Position;
     }

    public bool CanSeePlayer(int bounces = 0)
    {
        PhysicsDirectSpaceState2D spaceState = GetWorld2D().DirectSpaceState;
        PhysicsRayQueryParameters2D query;
        
        query = PhysicsRayQueryParameters2D.Create(Position, Player.Position, CollisionMask, new Array<Rid> { GetRid() });
        Dictionary results = spaceState.IntersectRay(query);
        if (results.Count == 0)
            return false;
        return results["rid"].AsRid() == Player.GetRid();
    }

    public abstract void SetGunRotationAndPosition(float radian = 0);
    public abstract void HandleAnimations();
}