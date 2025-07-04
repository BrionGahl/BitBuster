using BitBuster.component;
using BitBuster.entity.player;
using Godot;

namespace BitBuster.hud;

public partial class BulletCounter : Control
{

	private WeaponComponent _playerWeapon;
	private TextureRect _counter;
	
	public override void _Ready()
	{
		_playerWeapon = ((Player)GetTree().GetFirstNodeInGroup("player")).WeaponComponent;

		_counter = GetNode<TextureRect>("Counter");
		
		_counter.Size = new Vector2(_playerWeapon.BulletCount * 4, 4);

		_playerWeapon.BulletCountChange += OnBulletCountChange;
	}

	private void OnBulletCountChange(int count)
	{
		int trueCount = _playerWeapon.BulletCount - count;
		_counter.Visible = true;
		if (trueCount == 0)
			_counter.Visible = false;
		else
			_counter.Size = new Vector2(trueCount * 4, 4);
	}
}
