using BitBuster.component;
using BitBuster.entity;
using BitBuster.entity.player;
using BitBuster.item;
using BitBuster.resource;
using Godot;

namespace BitBuster.hud;

public partial class TitleBar : VSplitContainer
{
	private AnimationPlayer _animationPlayer;

	private Player _player;
	private EntityStats _playerStats;

	private Label _title;
	private Label _subtitle;
	
	public override void _Ready()
	{
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_player = (Player)GetTree().GetFirstNodeInGroup("player");
		_playerStats = _player.EntityStats;

		_title = GetNode<Label>("Title");
		_subtitle = GetNode<Label>("Subtitle");
		
		_playerStats.ItemAdded += OnItemAdded;
	}

	private void SetLabels(string title, string subtitle)
	{
		_title.Text = title;
		_subtitle.Text = subtitle;
		_animationPlayer.Play("effect_display");
	}
	
	private void OnItemAdded(Item item)
	{
		SetLabels(item.ItemName, item.ItemDescription);
	}
}
