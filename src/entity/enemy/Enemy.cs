using System;
using System.Collections.Generic;
using BitBuster.component;
using BitBuster.entity.player;
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
    protected Player _player;
    
    protected StatsComponent _statsComponent;
    protected HealthComponent _healthComponent;
    protected HitboxComponent _hitboxComponent;
    protected WeaponComponent _weaponComponent;

    protected Vector2 _spawnPosition;
    protected Vector2 _target;

    [Export]
    public EnemyState State { get; set; }
    
    public virtual void InitializeEnemy(Player player) // Make this a parent class
    {
        Logger.Log.Information("Linking Enemy's Children...");

        _player = player;
        
        _statsComponent = GetNodeOrNull<Node2D>("StatsComponent") as StatsComponent;
        _healthComponent = GetNodeOrNull<Node2D>("HealthComponent") as HealthComponent;
        _hitboxComponent = GetNodeOrNull<Node2D>("HitboxComponent") as HitboxComponent;
        _weaponComponent = GetNodeOrNull<Node2D>("WeaponComponent") as WeaponComponent;

        if (_healthComponent != null)
            _healthComponent.StatsComponent = _statsComponent;
        if (_hitboxComponent != null)
            _hitboxComponent.HealthComponent = _healthComponent;
        if (_weaponComponent != null)
            _weaponComponent.StatsComponent = _statsComponent;

        _spawnPosition = Position;

        State = EnemyState.Idle;
    }

    protected bool CanSeePlayer(int bounces = 0)
    {
        PhysicsDirectSpaceState2D spaceState = GetWorld2D().DirectSpaceState;
        PhysicsRayQueryParameters2D query;
        
        query = PhysicsRayQueryParameters2D.Create(Position, _player.Position, CollisionMask, new Array<Rid> { GetRid() });
        Dictionary results = spaceState.IntersectRay(query);
        if (results.Count == 0)
            return false;
        return results["rid"].AsRid() == _player.GetRid();
    }
}