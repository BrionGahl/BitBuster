using System;
using BitBuster.component;
using BitBuster.entity.player;
using BitBuster.utils;
using Godot;

namespace BitBuster.entity.enemy;

public enum EnemyState
{
    Idle = 0,
    Surround = 1,
    Pursue = 2,
    Evade = 3,
}

public abstract partial class Enemy: CharacterBody2D
{
    protected Player _player;
    
    protected StatsComponent _statsComponent;
    protected HealthComponent _healthComponent;
    protected HitboxComponent _hitboxComponent;
    protected WeaponComponent _weaponComponent;

    protected Vector2 _spawnPosition;

    [Export]
    public EnemyState State { get; set; } = EnemyState.Idle;
    
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
    }
}