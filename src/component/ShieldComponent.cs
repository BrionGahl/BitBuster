using Godot;

namespace BitBuster.component;

public partial class ShieldComponent : CharacterBody2D
{
	private HealthComponent _healthComponent;

	private Area2D _shield;
	private GpuParticles2D _gpuParticles;
	
	private float _speed = 100f;

	public override void _Ready()
	{
		_healthComponent = GetParent().GetNode<HealthComponent>("HealthComponent");
		
		_shield = GetNode<Area2D>("Area2D");
		_gpuParticles = GetNode<GpuParticles2D>("GPUParticles2D");
		
		_healthComponent.HealthIsZero += OnHealthIsZero;
	}
	
	public override void _PhysicsProcess(double delta)
	{
		RotationDegrees += _speed * (float)delta % 360f;
	}

	private void OnHealthIsZero()
	{
		QueueFree();
	}
}
