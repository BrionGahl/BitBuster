using System;
using BitBuster.component;
using BitBuster.Component;
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

    protected Vector2 _spawnPosition;

    [Export]
    public EnemyState State { get; set; } = EnemyState.Idle;
    
    public virtual void LinkNodes(Player player) // Make this a parent class
    {
        Logger.Log.Information("Linking Enemy's Children...");

        _player = player;
        
        _statsComponent = GetNode<Node2D>("StatsComponent") as StatsComponent;
        _healthComponent = GetNode<Node2D>("HealthComponent") as HealthComponent;
        _hitboxComponent = GetNode<Node2D>("HitboxComponent") as HitboxComponent;

        _healthComponent.LinkNodes();
        _hitboxComponent.LinkNodes();

        _spawnPosition = Position;
    }
}