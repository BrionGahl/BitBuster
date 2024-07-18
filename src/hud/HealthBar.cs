using BitBuster.component;
using Godot;

namespace BitBuster.hud;

public partial class HealthBar : Control
{

	private HealthComponent _playerHealthComponent;
	private TextureRect _healthEmpty;
	private TextureRect _healthFull;
	
	public override void _Ready()
	{
		_healthEmpty = GetNode<TextureRect>("HealthEmpty");
		_healthFull = GetNode<TextureRect>("HealthFull");

		_playerHealthComponent = GetTree().GetFirstNodeInGroup("player").GetNode("HealthComponent") as HealthComponent;

		_playerHealthComponent.HealthChange += OnHealthChange;
	}

	private void OnHealthChange(float value)
	{
		_healthEmpty.Size = new Vector2(_playerHealthComponent.MaxHealth * 16, 16);
		
		if (_playerHealthComponent.CurrentHealth <= 0)
			_healthFull.Visible = false;
		else
			_healthFull.Size = new Vector2(_playerHealthComponent.CurrentHealth * 16, 16);
	}
	
}
