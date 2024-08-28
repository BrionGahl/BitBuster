using BitBuster.component;
using BitBuster.data;
using BitBuster.utils;
using BitBuster.world;
using Godot;

namespace BitBuster.weapon;

public partial class EffectPool : Area2D
{
	// TODO: Instantiate pool somewhere, have a set position method.
	// From there based on effect deal damage to player with damageTimer every so often, and apply given effect.
	// If entity needs to respond to effect they do it on their end such as slowing.
	// On Pool side check based on effect how it should respond to certain events. Such as fire bullet detonating oil/poisons.
	
	private ExplodingComponent _explodingComponent;
	private StaticBody2D _smokeBody;
	
	private Timer _despawnTimer;
	private Timer _damageTimer;

	private GpuParticles2D _poolEmitter;
	private GpuParticles2D _poolSubEmitter;
	
	private bool _elementsResolved;

	public AttackData AttackData { get; set; }
	public bool Emitting
	{
		set
		{
			_poolEmitter.Emitting = value;
			_poolSubEmitter.Emitting = value;
		}
	}
	
	public override void _Ready()
	{
		_smokeBody = GetNode<StaticBody2D>("SmokeBody");
		
		_explodingComponent = GetNode<ExplodingComponent>("ExplodingComponent");
		_explodingComponent.SetRadius(30f);
		
		_despawnTimer = GetNode<Timer>("DespawnTimer");
		_damageTimer = GetNode<Timer>("DamageTimer");

		_poolEmitter = GetNode<GpuParticles2D>("PoolEmitter");
		_poolSubEmitter = GetNode<GpuParticles2D>("PoolSubEmitter");

		
		_despawnTimer.Timeout += OnDespawnTimeout;
		_explodingComponent.ExplodingEmitter.Finished += OnExplodeFinished;
	}

	public void SetPosition(Vector2 position, AttackData attackData)
	{
		Position = position;
		AttackData = attackData;

		Color color = Colors.Black;
		color.A = 0.5f;
		
		if (AttackData.Effects.HasFlag(EffectType.Fire))
		{
			color.R += 1;
			color.G += 0.35f;
		}

		if (AttackData.Effects.HasFlag(EffectType.Oil))
		{
			color = Colors.SaddleBrown;
		}
		
		if (AttackData.Effects.HasFlag(EffectType.Water))
		{
			color.B += 1;
		}

		Modulate = color;
	}
	
	public override void _Process(double delta)
	{
		CheckElementCombinations();
		
		foreach (var area in GetOverlappingAreas())
		{
			if (area.IsInGroup(Groups.GroupPool))
			{
				AttackData.Effects |= ((EffectPool)area).AttackData.Effects;
			}
		}
		
		foreach (var body in GetOverlappingBodies())
		{
			if (body.IsInGroup(Groups.GroupBullet))
			{
				AttackData.Effects |= ((Bullet)body).AttackData.Effects;
			}
		}

		if (_damageTimer.TimeLeft > 0)
			return;
		
		_damageTimer.Start();
		foreach (var area in GetOverlappingAreas())
		{
			if (area is not HitboxComponent hitboxComponent)
				return;
			
			HandleElement(hitboxComponent);
		}
		
	}


	private void HandleElement(HitboxComponent hitboxComponent)
	{
		hitboxComponent.ApplyStatus(AttackData.Effects);
	}
	
	private void CheckElementCombinations()
	{
		if (_elementsResolved)
			return;
		
		if (AttackData.Effects.HasFlag(EffectType.Explode))
		{
			Modulate = Colors.White;
			Emitting = false;
			
			_explodingComponent.Explode(new AttackData(1f, 0, SourceType.World, false));
			
			_elementsResolved = true;
		}
		
		if (AttackData.Effects.HasFlag(EffectType.Smoke))
		{
			Modulate = Colors.White;
			Emitting = true;

			_poolSubEmitter.Amount *= 5;
			_smokeBody.SetCollisionLayerValue((int)BbCollisionLayer.EntityNoSee, true);
			
			_elementsResolved = true;
		}

		if (AttackData.Effects.HasFlag(EffectType.Sludge))
		{
			Modulate = Colors.Purple;
			Emitting = true;
			
			_elementsResolved = true;
		}
		
	}
	
	private void OnDespawnTimeout()
	{
		QueueFree();
	}

	// private void OnDamageTimeout()
	// {
	// 	
	// }
	
	private void OnExplodeFinished()
	{
		QueueFree();
	}
}
