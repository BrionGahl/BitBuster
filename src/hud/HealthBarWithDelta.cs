using BitBuster.component;
using BitBuster.entity.player;
using BitBuster.utils;
using Godot;
using Serilog;

namespace BitBuster.hud;

public partial class HealthBarWithDelta : ProgressBar
{

	private HealthComponent _playerHealthComponent;

	private ProgressBar _deltaBar;
	private ProgressBar _overhealBar;
	
	private Timer _timer;
	private TextureRect _overlay;
	
	public override void _Ready()
	{
		_playerHealthComponent = ((Player)GetTree().GetFirstNodeInGroup("player")).HealthComponent;
		
		_deltaBar = GetNode<ProgressBar>("DeltaBar");
		_overhealBar = GetNode<ProgressBar>("OverhealBar");
		_timer = GetNode<Timer>("Timer");
		_overlay = GetNode<TextureRect>("Overlay");

		MaxValue = _playerHealthComponent.MaxHealth;
		Value = _playerHealthComponent.CurrentHealth;
		MinValue = 0;
		
		_deltaBar.MaxValue = _playerHealthComponent.MaxHealth;
		_deltaBar.Value = _playerHealthComponent.CurrentHealth;
		_deltaBar.MinValue = 0;

		_overhealBar.MaxValue = _playerHealthComponent.MaxHealth;
		_overhealBar.Value = _playerHealthComponent.Overheal;
		_overhealBar.MinValue = 0;
		
		_playerHealthComponent.HealthChange += OnHealthChange;
		_timer.Timeout += OnDeltaTimeout;
		
		_timer.Start(0.125);
	}
	
	private void OnHealthChange(float value)
	{
		MaxValue = _playerHealthComponent.MaxHealth;
		Value = _playerHealthComponent.CurrentHealth;

		_overhealBar.MaxValue = _playerHealthComponent.MaxHealth;
		_overhealBar.Value = _playerHealthComponent.Overheal;

		Size = new Vector2(_playerHealthComponent.MaxHealth * 16, 16);

		_timer.Start(0.333);
	}

	private void OnDeltaTimeout()
	{
		Size = new Vector2(_playerHealthComponent.MaxHealth * 16, 16);

		_deltaBar.MaxValue = _playerHealthComponent.MaxHealth;
		_deltaBar.Value = Value;
	}
}
