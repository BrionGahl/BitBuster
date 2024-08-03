using BitBuster.component;
using BitBuster.items;
using Godot;

namespace BitBuster.hud;

public partial class ItemsContainer : HFlowContainer
{
	private StatsComponent _playerStats;

	public override void _Ready()
	{
		_playerStats = GetTree().GetFirstNodeInGroup("player").GetNode<StatsComponent>("StatsComponent");
		_playerStats.ItemAdded += OnItemAdded;
	}

	private void OnItemAdded(Item item)
	{
		TextureRect newTextureRect = new TextureRect();
		newTextureRect.Texture = item.ItemTexture;
		AddChild(newTextureRect);
	}
}
