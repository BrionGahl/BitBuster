using Godot;
using System;
using BitBuster.entity.player;
using BitBuster.utils;

public partial class BulletCounter : Control
{

	private Weapon _playerWeapon;
	private TextureRect _counter;
	
	public override void _Ready()
	{
		_playerWeapon = GetNode<Node2D>("../../../Player/Weapon") as Weapon;
		_counter = GetNode<TextureRect>("Counter");
		
		_counter.Size = new Vector2(_playerWeapon.BulletCount * 4, 4);

		_playerWeapon.BulletCountChange += OnBulletCountChange;
	}

	private void OnBulletCountChange(int count)
	{
		int trueCount = _playerWeapon.BulletCount + 1 - count;
		_counter.Visible = true;
		if (trueCount == 0)
			_counter.Visible = false;
		else
			_counter.Size = new Vector2(trueCount * 4, 4);
	}




}
