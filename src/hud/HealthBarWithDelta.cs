using BitBuster.Component;
using Godot;

namespace BitBuster.hud;

public partial class HealthBarWithDelta : ProgressBar
{

	private HealthComponent _playerHealthComponent;

	private ProgressBar _deltaBar;
	private Timer _timer;
	private TextureRect _overlay;
	
	public override void _Ready()
	{
		_playerHealthComponent = GetNode<Node2D>("../../Player/HealthComponent") as HealthComponent;
		_deltaBar = GetNode<ProgressBar>("DeltaBar");
		_timer = GetNode<Timer>("Timer");
		_overlay = GetNode<TextureRect>("Overlay");

		MaxValue = _playerHealthComponent.MaxHealth;
		Value = _playerHealthComponent.CurrentHealth;
		MinValue = 0;
		
		_deltaBar.MaxValue = _playerHealthComponent.MaxHealth;
		_deltaBar.Value = _playerHealthComponent.CurrentHealth;
		_deltaBar.MinValue = 0;
		
		_playerHealthComponent.HealthChange += OnHealthChange;
		_timer.Timeout += OnDeltaTimeout;
	}
	
	private void OnHealthChange()
	{
		MaxValue = _playerHealthComponent.MaxHealth;
		Value = _playerHealthComponent.CurrentHealth;

		Size = new Vector2(_playerHealthComponent.MaxHealth * 16, 16);

		_timer.Start();
	}

	private void OnDeltaTimeout()
	{
		_deltaBar.MaxValue = _playerHealthComponent.MaxHealth;
		_deltaBar.Value = Value;
	}
	
}
