using BitBuster.entity.player;
using BitBuster.item;
using BitBuster.resource;
using Godot;

namespace BitBuster.hud;

public partial class ItemsContainer : HFlowContainer
{
	private EntityStats _playerStats;

	public override void _Ready()
	{
		_playerStats = ((Player)GetTree().GetFirstNodeInGroup("player")).EntityStats;
		_playerStats.ItemAdded += OnItemAdded;
	}

	private void OnItemAdded(Item item)
	{
		TextureRect newTextureRect = new TextureRect();
		newTextureRect.Texture = item.ItemTexture;
		AddChild(newTextureRect);
	}
}
