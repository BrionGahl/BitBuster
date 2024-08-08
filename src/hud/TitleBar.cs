using BitBuster.component;
using BitBuster.entity;
using BitBuster.entity.player;
using BitBuster.items;
using BitBuster.resource;
using Godot;

namespace BitBuster.hud;

public partial class TitleBar : VSplitContainer
{
	private AnimationPlayer _animationPlayer;
	private EntityStats _playerStats;

	private Label _title;
	private Label _subtitle;
	
	public override void _Ready()
	{
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_playerStats = ((Player)GetTree().GetFirstNodeInGroup("player")).EntityStats;

		_title = GetNode<Label>("Title");
		_subtitle = GetNode<Label>("Subtitle");
		
		_playerStats.ItemAdded += OnItemAdded;
	}

	private void SetLabels(string title, string subtitle)
	{
		_title.Text = title;
		_subtitle.Text = subtitle;
	}
	
	private void OnItemAdded(Item item)
	{
		SetLabels(item.ItemName, item.ItemDescription);
		_animationPlayer.Play("effect_display");
	}
}
